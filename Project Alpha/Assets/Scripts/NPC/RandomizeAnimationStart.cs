using UnityEngine;
using System.Collections;

public class RandomizeAnimationStart : MonoBehaviour {
	// Use this for initialization
	void Start () {
        Animator theAnimator = GetComponent<Animator>();
        theAnimator.speed = Random.Range(1.0f, 1.5f);
	}
}
