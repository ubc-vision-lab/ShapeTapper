using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractFixation : MonoBehaviour {

	public delegate void FixationComplete(); // callback when complete?

	public static event FixationComplete fixationComplete;

	protected AssetBundle assetBundle;
	protected GameObject fixationCross;
	protected GameObject fingerHome;
	protected float progressionTime;

	protected void Awake()
	{
		//TODO: Grab the configuration?
		assetBundle = GameObject.FindObjectOfType<ExperimentConfig>().assetBundle;
		fixationCross = assetBundle.LoadAsset<GameObject>("fixation_cross");
		fingerHome = assetBundle.LoadAsset<GameObject>("home_button");

		fixationCross.GetComponent<Renderer>().enabled = false;
		fingerHome.GetComponent<Renderer>().enabled = false;
	}

	private void Start()
	{
		ShowFixation();
	}

	protected virtual IEnumerator ShowFixation()
	{
		yield return new WaitForSecondsRealtime(progressionTime);
	}

	protected abstract IEnumerator ProgressFixation();

}