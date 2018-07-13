using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[RequireComponent(typeof(FixationManager))]
public class EmptyFixation : MonoBehaviour, IFixation
{
	private FixationManager fixationManager;
	private float trialOnset;
	private Coroutine progressFixationCoroutine;
	private bool progressFixationComplete = false;

	void Awake()
	{
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
		finally
		{
			Debug.Log("Trial to start in " + trialOnset + " seconds.");
		}
		
	}

	// Use this for initialization
	void Start () {
		ShowFixation();
		progressFixationCoroutine = StartCoroutine(ProgressFixation());
	}
	
	void Update () {
		if(!progressFixationComplete)
		{
			CompleteFixation();
		}
	}

	/// <summary>
	/// Show the fixation if it exists.
	/// </summary>
	public void ShowFixation()
	{
		// no fixation to show
	}

	/// <summary>
	/// Count the time before the stimuli are presented
	/// </summary>
	/// <returns></returns>
	public IEnumerator ProgressFixation()
	{
		progressFixationComplete = false;
		yield return new WaitForSecondsRealtime(trialOnset);
		fixationManager.OnPrimary();
		progressFixationComplete = true;
	}

	/// <summary>
	/// Nothing happens here.
	/// </summary>
	public void CompleteFixation()
	{
		// nothing happens here also
	}
}