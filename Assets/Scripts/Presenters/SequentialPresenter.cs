using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialPresenter : AbstractPresenter {

	// Use this for initialization
	public override void Present()
	{
		Debug.Log("Present called.");
		foreach (IStimulus stimulus in ToBePresented)
		{
			//TODO: Figure out how to make sequential work (probably coroutine)
			StartCoroutine(stimulus.Stimulate());
		}
	}

	// Unity setup functions are done in AbstractPresenter
}