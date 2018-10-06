using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class PointerTrigger : AbstractTrigger {

	private TrialDelegate trialDelegate;
	private float stimulusOnset;
	private float touchTime;

	private void Awake()
	{
		trialDelegate = FindObjectOfType<TrialDelegate>();
		stimulusOnset = ExperimentConfig.instance.GetCurrentConfig().TrialSetting._stimulus_onset;
	}

	private void OnMouseDown()
	{
		var pointerPress = "Pressed the Pointer Trigger!";
		Debug.Log(pointerPress);
		touchTime = Time.time;
		trialDelegate.OnReadyToStartTrial();
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
			var fingerLiftEarly = "Finger lifted from home button";
			trialDelegate.AbortTrial(fingerLiftEarly);
			Debug.Log(fingerLiftEarly);
		}
		else
		{
			Debug.Log("Finger lifted after flashing.");
			trialDelegate.OnReadyToPresentConditionalStimuli();
		}
		gameObject.GetComponent<Renderer>().enabled = false;
	}
}