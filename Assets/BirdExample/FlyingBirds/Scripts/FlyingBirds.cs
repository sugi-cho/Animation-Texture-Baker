using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class FlyingBirds : MonoBehaviour {
    public ComputeShader cs;

    public Mesh mesh;
    public Material mat;
    public uint numBirds;
    public float maxSpeed = 50f;
    public float targetSize = 10f;
    public float baseSpeed = 1.5f;
    public float speedRange = 0.5f;

    public Transform targetSource;

    Bounds bounds;
    ComputeBuffer argBuffer;

    ComputeBuffer birdBuffer;
    int initKernel;
    int updateKernel;

    Bird[] birdData;

    public void SetPosBuffer(ComputeBuffer buffer0, ComputeBuffer buffer1)
    {
        mat.SetBuffer("_PosBuffer", buffer0);
        mat.SetBuffer("_NormBuffer", buffer1);
    }

    public void SetBufferToCompute(ComputeBuffer buffer0, ComputeBuffer buffer1)
    {
        cs.SetBuffer(initKernel, "_ToPos", buffer0);
        cs.SetBuffer(initKernel, "_ToDir", buffer1);
        cs.SetBuffer(updateKernel, "_ToPos", buffer0);
        cs.SetBuffer(updateKernel, "_ToDir", buffer1);
    }
    
	// Use this for initialization
	void Awake () {
        bounds = new Bounds(Vector3.zero, Vector3.one * 100);
        argBuffer = new ComputeBuffer(5, sizeof(uint), ComputeBufferType.IndirectArguments);
        var args = new[] { mesh.GetIndexCount(0), numBirds, mesh.GetIndexStart(0), mesh.GetBaseVertex(0), (uint)0 };
        argBuffer.SetData(args);

        birdData = new Bird[numBirds];
        birdBuffer = new ComputeBuffer((int)numBirds, sizeof(float) * (3 + 3 + 4));
        mat.SetBuffer("_Bird", birdBuffer);

        initKernel = cs.FindKernel("init");
        updateKernel = cs.FindKernel("update");

        cs.SetBuffer(initKernel, "_Bird", birdBuffer);
        cs.SetBuffer(updateKernel, "_Bird", birdBuffer);

        cs.SetInt("_numBirds", (int)numBirds);
    }

    void UpdateBirds()
    {
        var dt = Time.deltaTime;
        cs.SetFloat("_maxSpeed", maxSpeed);
        cs.SetFloat("_targetSize", targetSize);
        cs.SetFloat("_baseSpeed", baseSpeed);
        cs.SetFloat("_speedRange", speedRange);
        cs.SetFloat("_dt", dt);
        cs.Dispatch(updateKernel, (int)numBirds / 8 + 1, 1, 1);
        birdBuffer.GetData(birdData);
    }

    private void OnDestroy()
    {
        new[] { argBuffer, birdBuffer }.ToList().ForEach(b => b.Dispose());
    }

    // Update is called once per frame
    void Update () {
        if (3f < targetSource.position.magnitude)
        {
            targetSource.position = Vector3.zero;
            targetSource.rotation = Quaternion.identity;
        }
        UpdateBirds();
        Graphics.DrawMeshInstancedIndirect(mesh, 0, mat, bounds, argBuffer, 0, null, ShadowCastingMode.On);
	}
    [System.Serializable]
    public struct Bird
    {
        public Vector3 pos;
        public Vector3 vel;
        public Quaternion rot;
    }
}
