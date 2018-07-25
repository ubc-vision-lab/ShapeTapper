using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashFixation : MonoBehaviour, IFixation {

	[SerializeField] GameObject fixationCross;
	GameObject targetFlash;
	TrialDelegate trialDelegate;
	float waitTime;
	readonly float flashTime = 0.03f; // two frames

	#region Unity functions
	private void Awake()
	{
		var assetBundle = GameObject.FindObjectOfType<ExperimentConfig>().assetBundle;
		var trialSetting = ExperimentConfig.instance.GetCurrentConfig().TrialSetting;
		trialDelegate = FindObjectOfType<TrialDelegate>();
		targetFlash = Instantiate(assetBundle.LoadAsset<GameObject>(ExperimentConfig.instance.GetCurrentConfig().GetTargetName()));
		targetFlash.GetComponent<Renderer>().transform.position = fixationCross.GetComponent<Renderer>().transform.position;
		targetFlash.GetComponent<Renderer>().enabled = false;
		waitTime = trialSetting._stimulus_onset;
	}

	void OnEnable()
	{
		TrialDelegate.ReadyToStartTrial += RunFixation;
		trialDelegate.FixationReady();
	}

	// Use this for initialization
	void Start () {
		ShowFixation();
		// now we wait for any notifications that we can start
	}

	void OnDisable()
	{
		TrialDelegate.ReadyToStartTrial -= RunFixation;
	}
	#endregion

	public IEnumerator RunFixation()
	{
		yield return ProgressFixation();
		CompleteFixation();
	}

	public void ShowFixation()
	{
		fixationCross.GetComponent<Renderer>().enabled = true;
	}

	public IEnumerator ProgressFixation()
	{
		yield return new WaitForSecondsRealtime(waitTime);
		fixationCross.GetComponent<Renderer>().enabled = false;
		targetFlash.GetComponent<Renderer>().enabled = true;
		yield return new WaitForSecondsRealtime(flashTime);
		targetFlash.GetComponent<Renderer>().enabled = false;
		trialDelegate.OnReadyToPresentStimuli();
	}

	public void CompleteFixation()
	{

	}
}
