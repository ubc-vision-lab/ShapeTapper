using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashFixation : MonoBehaviour, IFixation {

	[SerializeField] GameObject fixationCross;
	GameObject targetFlash;
	[SerializeField] TrialDelegate trialDelegate;
	Coroutine progressFixationCoroutine;
	float waitTime;
	[SerializeField] float flashTime = 0.05f; // three frames @ 60fps
	[SerializeField] float preStimulusTime = 0.1f;

	bool fixationComplete = false;

	public bool FixationComplete
	{
		get
		{
			return fixationComplete;
		}

		set
		{
			fixationComplete = value;
		}
	}

	#region Unity functions
	private void Awake()
	{
		var assetBundle = GameObject.FindObjectOfType<ExperimentConfig>().assetBundle;
		var trialSetting = ExperimentConfig.instance.GetCurrentConfig().TrialSetting;
		targetFlash = Instantiate(assetBundle.LoadAsset<GameObject>(ExperimentConfig.instance.GetCurrentConfig().GetTargetName()), fixationCross.transform);
		targetFlash.GetComponent<Renderer>().enabled = false;
		targetFlash.GetComponent<Transform>().localPosition = new Vector3(0, 0, 1);
		waitTime = trialSetting._stimulus_onset;
	}

	void OnEnable()
	{
		TrialDelegate.ReadyToStartTrial += RunFixation;
		trialDelegate.FixationReady();
		TrialDelegate.ReadyForFeedback += TerminateFixation;
	}

	// Use this for initialization
	void Start () {
		ShowFixation();
		// now we wait for any notifications that we can start
	}

	void OnDisable()
	{
		TrialDelegate.ReadyToStartTrial -= RunFixation;
		TrialDelegate.ReadyForFeedback -= TerminateFixation;
	}
	#endregion

	public void RunFixation()
	{
		Debug.Log("FlashFixation.RunFixation");
		progressFixationCoroutine = StartCoroutine(ProgressFixation());
	}

	public void ShowFixation()
	{
		Debug.Log("FlashFixation.ShowFixation");
		fixationCross.GetComponent<Renderer>().enabled = true;
	}

	public IEnumerator ProgressFixation()
	{
		Debug.Log("FlashFixation.ProgressFixation");
		yield return new WaitForSecondsRealtime(waitTime);
		Debug.Log("Flashing Fixation");
		fixationCross.GetComponent<Renderer>().enabled = false;
		targetFlash.GetComponent<Renderer>().enabled = true;
		yield return new WaitForSecondsRealtime(flashTime);
		Debug.Log("Fixation Flashed. Sending command to present Stimuli.");
		targetFlash.GetComponent<Renderer>().enabled = false;
		yield return new WaitForSecondsRealtime(preStimulusTime);
		trialDelegate.OnReadyToPresentStimuli();
	}

	public void CompleteFixation()
	{
		FixationComplete = true;
	}

	public void TerminateFixation()
	{
		if(progressFixationCoroutine != null)
		{
			StopCoroutine(progressFixationCoroutine);
		}
	}
}