using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimultaneousPresenter : AbstractPresenter
{
	public override void Present()
	{
		presentCalled = true;
		foreach (IStimulus stimulus in ToBePresented)
		{
			stimulusCoroutines.Add(StartCoroutine(stimulus.Stimulate()));
		}
	}

	private void Update()
	{
		// if the present function is already called, then
		if(presentCalled && !promptCalled)
		{
			var completeCoroutines = 0;
			foreach(IStimulus stimulus in ToBePresented)
			{
				if(stimulus.CoroutineIsFinished())
				{
					completeCoroutines++;
					continue;
				} else
				{
					break;
				}
			}
			if(completeCoroutines == ToBePresented.Count)
			{
				// we're done!
				promptCalled = true;
				trialDelegate.OnReadyForPrompt();
			}

		}
	}
	// Unity setup functions are done in AbstractPresenter
}