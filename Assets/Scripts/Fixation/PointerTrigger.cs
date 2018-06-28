using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTrigger : FixationTrigger {

	private AbstractFixation fixation;

	// Use this for initialization
	void Start () {
		fixation = GameObject.FindObjectOfType<AbstractFixation>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
