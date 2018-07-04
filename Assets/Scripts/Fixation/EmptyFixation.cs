using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NoFixation : AbstractFixation {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected override IEnumerator ProgressFixation()
	{
		throw new NotImplementedException();

	}
}