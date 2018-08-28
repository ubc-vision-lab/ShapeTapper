using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPresenter : MonoBehaviour
{
	protected TrialDelegate trialDelegate;
	public static List<IStimulus> ToBePresented = new List<IStimulus>();
	protected List<Coroutine> stimulusCoroutines = new List<Coroutine>();
	public Vector3 touch;
	public float touchTime;
	protected bool presentCalled = false;
	protected bool promptCalled = false;

	// Runs the behaviour of the fixation
	public abstract void Present();

	protected virtual void OnEnable()
	{
		trialDelegate = Object.FindObjectOfType<TrialDelegate>();
		if(trialDelegate != null)
		{
			TrialDelegate.ReadyToPresentStimuli += Present;
		}
		else { return; }
		trialDelegate.StimulusReady();
	}

	protected virtual void OnDisable()
	{
		if (trialDelegate != null)
		{
			TrialDelegate.ReadyToPresentStimuli -= Present;
		}
		else { return; }
	}
}