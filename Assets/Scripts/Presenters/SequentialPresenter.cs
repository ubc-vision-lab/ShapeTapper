using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SequentialPresenter : AbstractPresenter {

	// Use this for initialization
	public override IEnumerator Present()
	{
		foreach (IStimulus stimulus in ToBePresented)
		{
			yield return stimulus.Stimulate();
		}
	}

	// Unity setup functions are done in AbstractPresenter
}
