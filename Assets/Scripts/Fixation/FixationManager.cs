using EnnsLab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationManager : MonoBehaviour {

	public delegate void FixationEvent(); // callback when complete?

	// Events
	public static event FixationEvent Primary; // to show first part of fixation
	public static event FixationEvent Secondary; // second have of stimulus set

	[SerializeField] GameObject emptyFixation;
	[SerializeField] GameObject streamFixation;
	[SerializeField] GameObject flashFixation;
	[SerializeField] GameObject pointerTrigger;

	// private fields
	TrialSetting trialSetting;

	void Awake()
	{
		SelectFixation();
	}

	private void Start()
	{

	}

	/// <summary>
	/// Creates the correct Fixations corresponding to the TrialSetting.
	/// </summary>
	void SelectFixation()
	{
		trialSetting = ExperimentConfig.instance.GetCurrentConfig().TrialSetting;
		switch (trialSetting._exp_mode)
		{
			case ((int)TrialSetting.ExpMode.spider):
				// load the home button and the fixation cross
				streamFixation.SetActive(true);
				pointerTrigger.SetActive(true);
				break;
			case ((int)TrialSetting.ExpMode.flasher):
				flashFixation.SetActive(true);
				pointerTrigger.SetActive(true);
				break;
			default: // default to original regardless
				gameObject.AddComponent<EmptyFixation>();
				break;
		}
	}

	/**
	 * The initial condition to begin presenting the stimulus was satisfied
	 */
	public void OnPrimary()
	{
		if (Primary != null)
		{
			Primary.Invoke();
		}
		else { return; }
	}

	public void OnSecondary()
	{
		if (Secondary != null)
		{
			Secondary.Invoke();
		}
		else { return; }
	}
}