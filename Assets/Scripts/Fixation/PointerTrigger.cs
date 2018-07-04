using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerTrigger : FixationTrigger {

	private AbstractFixation fixation;

	private void Awake()
	{
		fixation = GameObject.FindObjectOfType<AbstractFixation>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}