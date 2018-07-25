using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPresenter : MonoBehaviour
{
	private TrialDelegate trialManager;
	public static List<IStimulus> ToBePresented = new List<IStimulus>();
	protected List<Coroutine> stimulusCoroutines = new List<Coroutine>();

	// Runs the behaviour of the fixation
	public abstract IEnumerator Present();

	protected virtual void OnEnable()
	{
		trialManager = Object.FindObjectOfType<TrialDelegate>();
		if(trialManager != null)
		{
			TrialDelegate.ReadyToPresentStimuli += Present;
		}
		else { return; }
		trialManager.StimulusReady();
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