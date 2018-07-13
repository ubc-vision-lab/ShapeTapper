using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTrigger : FixationTrigger {

	private FixationManager fixation;

	private void Awake()
	{
		fixation = GameObject.FindObjectOfType<FixationManager>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}