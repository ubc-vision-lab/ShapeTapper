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

	public void RunFixation()
	{
		progressFixationCoroutine = StartCoroutine(ProgressFixation());
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
		if(progressFixationComplete)
		{
			Debug.Log("ProgressFixation already run once! Not running again =.=");
			yield return null;
		}
		else
		{
			Debug.Log("EmptyFixation.ProgressFixation");
			progressFixationComplete = false;
			yield return new WaitForSecondsRealtime(trialOnset);
			fixationManager.OnPrimary();
			progressFixationComplete = true;
			CompleteFixation();
		}
	}

	/// <summary>
	/// Nothing happens here.
	/// </summary>
	public void CompleteFixation()
	{
		Debug.Log("EmptyFixation.CompleteFixation");
		// nothing happens here also
	}

	public void TerminateFixation()
	{
		if(progressFixationCoroutine != null)
		{
			StopCoroutine(progressFixationCoroutine);
		}
	}
}