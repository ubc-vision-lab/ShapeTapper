using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PresenterManager : MonoBehaviour {

	void Awake()
	{
		DeterminePresenter();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void DeterminePresenter()
	{
		var trialSetting = ExperimentConfig.instance.GetCurrentConfig().TrialSetting;
		switch(trialSetting._exp_mode)
		{
			case ((int)EnnsLab.TrialSetting.ExpMode.flasher):
			case ((int)EnnsLab.TrialSetting.ExpMode.oddball):
			case ((int)EnnsLab.TrialSetting.ExpMode.same_different):
				gameObject.GetComponent<SimultaneousPresenter>().enabled = true;
				break;
			case ((int)EnnsLab.TrialSetting.ExpMode.spider):
				gameObject.GetComponent<ConditionalSequentialPresenter>().enabled = true;
				break;
			default:
				gameObject.GetComponent<SequentialPresenter>().enabled = true;
				break;
		}
	}
}
