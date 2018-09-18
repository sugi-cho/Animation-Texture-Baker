using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MeshPositionCapture : MonoBehaviour
{
    public Shader writer;
    public SkinnedMeshRenderer targetRenderer;
    public BufferEvent onBufferCreate;
    ComputeBuffer positionBuffer;
    ComputeBuffer normalBuffer;
    Camera cam;

    // Use this for initialization
    void Start()
    {
        var mesh = targetRenderer.sharedMesh;
        var vCount = mesh.vertexCount;
        positionBuffer = new ComputeBuffer(vCount, sizeof(float) * 3);
        normalBuffer = new ComputeBuffer(vCount, sizeof(float) * 3);
        onBufferCreate.Invoke(positionBuffer, normalBuffer);
        targetRenderer.material.SetOverrideTag("BufferWrite", "WriteSource");

        cam = new GameObject("_cam").AddComponent<Camera>();
        cam.clearFlags = CameraClearFlags.Nothing;
        cam.orthographic = true;
        cam.nearClipPlane = -100;
        cam.farClipPlane = 100;
        cam.orthographicSize = 100;
        cam.enabled = false;
        var posRenderer = cam.gameObject.AddComponent<PosRenderer>();
        posRenderer.target = targetRenderer;
    }

    private void OnDestroy()
    {
        if (positionBuffer != null)
            positionBuffer.Dispose();
        if (normalBuffer != null)
            normalBuffer.Dispose();
    }

    void LateUpdate()
    {
        Graphics.SetRandomWriteTarget(1, positionBuffer);
        Graphics.SetRandomWriteTarget(2, normalBuffer);
        cam.RenderWithShader(writer, "BufferWrite");
        Graphics.ClearRandomWriteTargets();
        targetRenderer.enabled = false;
    }

    [System.Serializable]
    public class BufferEvent : UnityEvent<ComputeBuffer, ComputeBuffer> { }
}
