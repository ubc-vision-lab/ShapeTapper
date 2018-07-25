using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EmptyFixation : MonoBehaviour, IFixation
{
	private FixationManager fixationManager;
	private float trialOnset;

	private Coroutine progressFixationCoroutine;
	private bool progressFixationComplete = false;

	void Awake()
	{
		fixationManager = gameObject.GetComponentInParent<FixationManager>();
		var assetBundle = GameObject.FindObjectOfType<ExperimentConfig>().assetBundle;
		try
		{
			var trialSetting = ExperimentConfig.instance.GetCurrentConfig().TrialSetting;
			trialOnset = trialSetting._stimulus_onset;
		}
		catch (Exception e)
		{
			Debug.LogException(e);
			trialOnset = 0.5f;
		}
	}

	void OnEnable()
	{
		Debug.Log("Trial uses Empty Fixation.");
		Debug.Log("Trial to start in " + trialOnset + " seconds.");

	}

	// Use this for initialization
	void Start () {
		ShowFixation();
	}
	
	void Update () {
	}

	public IEnumerator RunFixation()
	{
		progressFixationCoroutine = StartCoroutine(ProgressFixation());
		yield return progressFixationCoroutine;
	}

	/// <summary>
	/// Show the fixation if it exists.
	/// </summary>
	public void ShowFixation()
	{
		Debug.Log("EmptyFixation.ShowFixation");
		// no fixation to show
	}

	/// <summary>
	/// Count the time before the stimuli are presented
	/// </summary>
	/// <returns></returns>
	public IEnumerator ProgressFixation()
	{
		Debug.Log("EmptyFixation.ProgressFixation");
		progressFixationComplete = false;
		yield return new WaitForSecondsRealtime(trialOnset);
		fixationManager.OnPrimary();
		progressFixationComplete = true;
		CompleteFixation();
	}

	/// <summary>
	/// Nothing happens here.
	/// </summary>
	public void CompleteFixation()
	{
		Debug.Log("EmptyFixation.CompleteFixation");
		// nothing happens here also
	}
}