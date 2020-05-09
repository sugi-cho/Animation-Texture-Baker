using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class VertIdxToUv : MonoBehaviour
{
    public int channel = 2;

    // Start is called before the first frame update
    void Start()
    {
        var mesh = GetComponent<MeshFilter>().mesh;
        mesh.hideFlags = HideFlags.DontSave;
        var uv = new List<Vector2>();
        var count = mesh.vertexCount;
        for (var i = 0; i < count; i++)
            uv.Add(new Vector2(i + 0.5f, 0) / Mathf.NextPowerOfTwo(count));
        mesh.SetUVs(channel, uv);
    }
}
