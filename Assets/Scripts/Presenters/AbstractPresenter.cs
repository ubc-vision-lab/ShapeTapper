using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPresenter : MonoBehaviour
{
	private TrialDelegate trialManager;
	public static List<IStimulus> ToBePresented = new List<IStimulus>();
	// Runs the behaviour of the fixation
	public virtual IEnumerator Present() {
		foreach(IStimulus stimulus in ToBePresented)
		{
			yield return stimulus.Stimulate();
		}
	}

	protected virtual void OnEnable()
	{
		trialManager = Object.FindObjectOfType<TrialDelegate>();
		if(trialManager != null)
		{
			TrialDelegate.ReadyToPresentStimuli += Present;
		}
		else { return; }
	}

	protected virtual void OnDisable()
	{
		if (trialManager != null)
		{
			TrialDelegate.ReadyToPresentStimuli -= Present;
		}
		else { return; }
	}
}