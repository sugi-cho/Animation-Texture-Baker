using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

using System.IO;


public class LineTextureBaker : EditorWindow
{
    public struct LineInfo
    {
        public LineInfo(int i0, int i1)
        {
            if (i0 < i1)
            {
                idx0 = i0;
                idx1 = i1;
                return;
            }
            idx0 = i1;
            idx1 = i0;
        }
        public override bool Equals(object obj)
        {
            if (obj == null || this.GetType() != obj.GetType())
                return false;

            var info = (LineInfo)obj;
            return idx0 == info.idx0 && idx1 == info.idx1;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public int idx0;
        public int idx1;
    }

    [MenuItem("Window/LineTextureBaker")]
    public static void CreateWindow()
    {
        GetWindow<LineTextureBaker>();
    }

    public Mesh targetMesh;
    public ComputeShader lineTexGen;
    RenderTexture output;

    private void OnGUI()
    {
        targetMesh = (Mesh)EditorGUILayout.ObjectField("mesh", targetMesh, typeof(Mesh), false);
        lineTexGen = (ComputeShader)EditorGUILayout.ObjectField("lineTexGenCS", lineTexGen, typeof(ComputeShader), false);
        if (GUILayout.Button("Bake Texture"))
            BakeTexture(targetMesh);
        if (GUILayout.Button("Create Mesh"))
            CreateMesh();
    }

    void BakeTexture(Mesh mesh)
    {
        var vs = mesh.vertices;
        var tris = mesh.triangles;
        var lines = new List<LineInfo>();

        for (var i = 0; i < tris.Length / 3; i++)
        {
            var idx0 = tris[i * 3 + 0];
            var idx1 = tris[i * 3 + 1];
            var idx2 = tris[i * 3 + 2];

            var l0 = new LineInfo(idx0, idx1);
            var l1 = new LineInfo(idx1, idx2);
            var l2 = new LineInfo(idx2, idx0);
            if (!lines.Contains(l0))
                lines.Add(l0);
            if (!lines.Contains(l1))
                lines.Add(l1);
            if (!lines.Contains(l2))
                lines.Add(l2);
        }
        var ls = lines.ToArray();
        var vBuffer = new ComputeBuffer(vs.Length, System.Runtime.InteropServices.Marshal.SizeOf(typeof(Vector3)));
        var lBuffer = new ComputeBuffer(ls.Length, System.Runtime.InteropServices.Marshal.SizeOf(typeof(LineInfo)));
        vBuffer.SetData(vs);
        lBuffer.SetData(ls);

        output = new RenderTexture(Mathf.NextPowerOfTwo(ls.Length), 2, 0, RenderTextureFormat.ARGBHalf);
        output.enableRandomWrite = true;
        output.Create();

        var kernel = lineTexGen.FindKernel("CSMain");
        lineTexGen.SetBuffer(kernel, "_VBuffer", vBuffer);
        lineTexGen.SetBuffer(kernel, "_LBuffer", lBuffer);
        lineTexGen.SetTexture(kernel, "Output", output);
        uint x, y, z;
        lineTexGen.GetKernelThreadGroupSizes(kernel, out x, out y, out z);
        lineTexGen.Dispatch(kernel, (int)(ls.Length / x) + 1, 1, 1);

        vBuffer.Release();
        lBuffer.Release();

        var folderName = "BakedLineTex";
        var folderPath = Path.Combine("Assets", folderName);
        if (!AssetDatabase.IsValidFolder(folderPath))
            AssetDatabase.CreateFolder("Assets", folderName);

        var tex2d = RenderTextureToTexture2D.Convert(output);
        AssetDatabase.CreateAsset(tex2d, Path.Combine(folderPath, mesh.name + string.Format("_line.{0}.asset", ls.Length)));
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = tex2d;
    }

    public static void CreateMesh()
    {
        var mesh = new Mesh();
        var numPoints = 65000;
        mesh.vertices = Enumerable.Repeat(Vector3.zero, numPoints).ToArray();
        mesh.uv = Enumerable.Range(0, numPoints).Select(i => new Vector2((float)i, (float)i / (float)numPoints)).ToArray();
        mesh.SetIndices(Enumerable.Range(0, numPoints).ToArray(), MeshTopology.Points, 0);

        AssetDatabase.CreateAsset(mesh, "Assets/point65000.asset");
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Selection.activeObject = mesh;
    }
}
