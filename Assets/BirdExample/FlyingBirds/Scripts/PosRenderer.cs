using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PosRenderer : MonoBehaviour {
    public Renderer target;
    private void OnPreCull()
    {
        target.enabled = true;
    }
    private void OnPostRender()
    {
        target.enabled = false;
    }
}
