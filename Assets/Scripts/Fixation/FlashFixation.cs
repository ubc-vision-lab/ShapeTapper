using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashFixation : MonoBehaviour, IFixation {

	private GameObject fixationCross;
	private GameObject fingerHome;
	private GameObject targetFlash;

	private void Awake()
	{
		var assetBundle = GameObject.FindObjectOfType<ExperimentConfig>().assetBundle;
		var trialSetting = ExperimentConfig.instance.GetCurrentConfig().TrialSetting;
		if(trialSetting._exp_mode == 2)
		{
			targetFlash = assetBundle.LoadAsset<GameObject>(ExperimentConfig.instance.GetCurrentConfig().GetTargetName());
		}
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void ShowFixation()
	{
		fixationCross.GetComponent<Renderer>().enabled = true;
		fingerHome.GetComponent<Renderer>().enabled = true;
	}

	public IEnumerator ProgressFixation()
	{
		throw new NotImplementedException();
	}

	public void CompleteFixation()
	{

	}
}
