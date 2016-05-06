using UnityEngine;
using System.Collections;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class AnimationTextureWriter : MonoBehaviour
{
    static Material animToTexMat
    {
        get { if (_mat == null) _mat = new Material(Shader.Find("Hidden/AnimToTexture")); return _mat; }
    }
    static Material _mat;

#if UNITY_EDITOR
    [MenuItem("Assets/Create/AnimationTexture/fps:10")]
    public static void CreateAnimationTexFromSelections10()
    {
        CreateAnimationTexFromSelections(10);
    }
    [MenuItem("Assets/Create/AnimationTexture/fps:30")]
    public static void CreateAnimationTexFromSelections30()
    {
        CreateAnimationTexFromSelections(30);
    }

    static void CreateAnimationTexFromSelections(int fps)
    {
        if (!EditorApplication.isPlaying)
        {
            EditorApplication.isPlaying = true;
            Debug.LogWarning("this function for play mode only");
            return;
        }
        foreach (var go in Selection.gameObjects)
        {
            var skin = go.GetComponentInChildren<SkinnedMeshRenderer>();
            var anim = go.GetComponentInChildren<Animation>();
            if (anim == null || skin == null)
            {
                continue;
            }
            //CreateTexture(skin, anim, fps);
            anim.gameObject.AddComponent<MonoBehaviour>().StartCoroutine(CreateTexture(skin, anim, fps));
        }
    }
    static IEnumerator CreateTexture(SkinnedMeshRenderer skin, Animation anim, int fps)
    {
        AssetDatabase.CreateFolder("Assets", anim.name + "_AnimationTexture");
        var folderPath = "Assets/" + anim.name + "_AnimationTexture/";

        foreach (var r in FindObjectsOfType<Renderer>())
            r.enabled = false;
        skin.enabled = true;
        skin.sharedMaterial = animToTexMat;
        var mesh = skin.sharedMesh;
        var material = skin.sharedMaterial;
        var dt = 1f / (float)fps;
        var width = Mathf.NextPowerOfTwo(mesh.vertexCount);
        var cam = Camera.main;
        cam.hdr = true;

        foreach (AnimationState state in anim)
        {
            var frames = Mathf.FloorToInt(state.length / dt);
            if (frames < 1)
                continue;
            var height = Mathf.NextPowerOfTwo(frames + 1);
            //rt is to create clean renderTexture.
            var rt = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBHalf);
            var rtp = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBHalf);
            var rtn = new RenderTexture(width, height, 0, RenderTextureFormat.ARGBHalf);

            RenderTexture.active = rtp;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rtn;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = null;
            rtp.name = anim.name + "_" + state.name + "_Pos_" + frames + "_" + fps;
            rtn.name = anim.name + "_" + state.name + "_Nml_" + frames + "_" + fps;

            state.speed = 0;
            cam.clearFlags = CameraClearFlags.Nothing;
            cam.targetTexture = rtp;
            cam.SetTargetBuffers(new[] { rt.colorBuffer, rtp.colorBuffer, rtn.colorBuffer }, rt.depthBuffer);

            for (var i = 0; i <= frames; i++)
            {
                material.SetInt("_Frame", i);
                state.time = ((float)i * dt) % state.length;
                anim.Play(state.name);
                yield return 0;
            }
            var tex2dp = RenderTextureToTexture2D.Convert(rtp);
            AssetDatabase.CreateAsset(tex2dp, folderPath + rtp.name + ".asset");
            var tex2dn = RenderTextureToTexture2D.Convert(rtn);
            AssetDatabase.CreateAsset(tex2dn, folderPath + rtn.name + ".asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        cam.targetTexture = null;
    }
#endif

}
