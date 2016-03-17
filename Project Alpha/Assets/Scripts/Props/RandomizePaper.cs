using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
public class RandomizePaper : MonoBehaviour {

    public Sprite[] Papers;
	// Use this for initialization
	void Start () {
        int randonm = Random.Range(0, Papers.Length);
        GetComponent<SpriteRenderer>().sprite = Papers[randonm];
	}
}
