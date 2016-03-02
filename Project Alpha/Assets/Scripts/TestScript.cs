using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TestScript : MonoBehaviour {
    public float i;
    private float b;

    public Vector3 haha;

    public List<Vector3> haList;

    public GameObject prefab;

	// Use this for initialization
     [AdvancedInspector.Inspect]
	void Start () {
        Instantiate(prefab);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
