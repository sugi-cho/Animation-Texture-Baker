using UnityEngine;

public class SetBufferBridge : MonoBehaviour
{
    public string propName0 = "_PosBuffer";
    public string propName1 = "_NormBuffer";
    public void SetBuffer(ComputeBuffer buffer)
    {
        GetComponent<Renderer>().material.SetBuffer(propName0, buffer);
    }
    public void SetBuffer(ComputeBuffer buffer0, ComputeBuffer buffer1)
    {
        GetComponent<Renderer>().material.SetBuffer(propName0, buffer0);
        GetComponent<Renderer>().material.SetBuffer(propName1, buffer1);
    }
}
