using EnnsLab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FixationManager : MonoBehaviour {

	public delegate void FixationEvent(); // callback when complete?

	// Events
	public static event FixationEvent Primary; // to show first part of fixation
	public static event FixationEvent Secondary; // 

	protected AssetBundle assetBundle;
	protected Coroutine progressCoroutine;
	protected bool progressCoroutineRunning;

	/**
	 * The initial condition to begin presenting the stimulus was satisfied
	 */
	public void OnPrimary()
	{
		Primary();
	}

	public void OnSecondary()
	{
		Secondary();
	}
}