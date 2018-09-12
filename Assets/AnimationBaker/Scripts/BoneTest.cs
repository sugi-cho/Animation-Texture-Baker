using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoneTest : MonoBehaviour {

    public Transform bone;
    public Matrix4x4 bindPose;
    public Matrix4x4 pose;

	// Use this for initialization
	void Start () {
        bindPose = bone.worldToLocalMatrix * transform.localToWorldMatrix;
	}
	
	// Update is called once per frame
	void Update () {
        pose = bone.worldToLocalMatrix * transform.localToWorldMatrix;
	}
}
