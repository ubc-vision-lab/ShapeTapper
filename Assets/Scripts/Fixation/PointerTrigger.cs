using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PointerTrigger : AbstractTrigger {

	private FixationManager fixationManager;
	private TrialDelegate trialDelegate;
	private float stimulusOnset;
	private float touchTime;

	private void Awake()
	{
		fixationManager = FindObjectOfType<FixationManager>();
		trialDelegate = FindObjectOfType<TrialDelegate>();
		stimulusOnset = ExperimentConfig.instance.GetCurrentConfig().TrialSetting._stimulus_onset;
	}

	private void OnMouseDown()
	{
		var pointerPress = "Pressed the Pointer Trigger!"
		Debug.Log(pointerPress);
		touchTime = Time.time;
	}

	private void OnMouseExit()
	{

		// Lift too early
		if(Time.time - touchTime < stimulusOnset)
		{
			trialDelegate.AbortTrial("Finger left home button");
		}
	}

	private void OnMouseUp()
	{
		if (Time.time - touchTime < stimulusOnset)
		{
			var fingerLiftEarly = "Finger left home button";
			trialDelegate.AbortTrial(fingerLiftEarly);
			Debug.Log(fingerLiftEarly);
		}
		else
		{
			trialDelegate.OnReadyToPresentConditionalStimuli();
		}
	}
}