using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashFixation : AbstractFixation {

	private GameObject fixationCross;
	private GameObject fingerHome;

	private void Awake()
	{
		assetBundle = GameObject.FindObjectOfType<ExperimentConfig>().assetBundle;
		var trialSetting = ExperimentConfig.instance.GetCurrentConfig().TrialSetting;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	protected override void ShowFixation()
	{
		fixationCross.GetComponent<Renderer>().enabled = true;
		fingerHome.GetComponent<Renderer>().enabled = true;
	}

	protected override IEnumerator ProgressFixation()
	{
		throw new NotImplementedException();
	}
}
