﻿using UnityEngine;
using System.Collections;

public class LockRotation : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
    //void Update () {
    //    this.transform.rotation = Quaternion.identity;
    //}

    void LateUpdate()
    {
        this.transform.rotation = Quaternion.identity;
    }
}