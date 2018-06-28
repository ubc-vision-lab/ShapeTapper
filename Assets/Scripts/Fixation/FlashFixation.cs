using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashFixation : AbstractFixation {

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

	protected override IEnumerator ShowFixation()
	{
		// waitforit
		throw new NotImplementedException();
	}
}
