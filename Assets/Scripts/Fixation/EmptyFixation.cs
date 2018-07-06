using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class EmptyFixation : AbstractFixation {

	private float trialOnset;

	void Awake()
	{
		assetBundle = GameObject.FindObjectOfType<ExperimentConfig>().assetBundle;
		var trialSetting = ExperimentConfig.instance.GetCurrentConfig().TrialSetting;
	}

	// Use this for initialization
	void Start () {
		ShowFixation();
		progressCoroutine = StartCoroutine(ProgressFixation());
	}
	
	// Update is called once per frame
	void Update () {
		if(!progressCoroutineRunning)
		{
			CompleteFixation();
		}
	}

	protected override void ShowFixation()
	{
		// no fixation to show
	}

	protected override IEnumerator ProgressFixation()
	{
		progressCoroutineRunning = true;
		yield return new WaitForSecondsRealtime(trialOnset);
		OnPrimary();
		progressCoroutineRunning = false;
	}
}