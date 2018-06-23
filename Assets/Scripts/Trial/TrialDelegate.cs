using EnnsLab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrialDelegate : MonoBehaviour {

	public delegate IEnumerator TrialEvent();

	public static event TrialEvent ReadyToStartTrial;
	public static event TrialEvent ReadyToPresentStimuli;
	public static event TrialEvent ReadyForPrompt;

	public AbstractPresenter fixation;
	public AbstractPrompt prompt;

	#region Fields
	private AbstractPresenter _trialFixation;
	private AbstractPresenter _stimulusPresenter;
	private string _assetPath;
	private AssetBundle _shapeAssetBundle;
	#endregion Fields

	#region Properties
	public AbstractPresenter TrialFixation
	{
		get
		{
			return _trialFixation;
		}

		set
		{
			_trialFixation = value;
		}
	}

	public AbstractPresenter StimulusManager
	{
		get
		{
			return _stimulusPresenter;
		}

		set
		{
			_stimulusPresenter = value;
		}
	}

	public string AssetPath
	{
		get
		{
			return _assetPath;
		}

		set
		{
			_assetPath = value;
		}
	}

	public AssetBundle ShapeAssetBundle
	{
		get
		{
			return _shapeAssetBundle;
		}

		set
		{
			_shapeAssetBundle = value;
		}
	}
	#endregion Properties

	// Use this for initialization
	void Start () {
		
	}

	protected IEnumerator OnReadyToStartTrial()
	{
		return ReadyToStartTrial?.Invoke();
	}

	protected IEnumerator OnReadyToPresentStimuli()
	{
		return ReadyToPresentStimuli?.Invoke();
	}

	protected IEnumerator OnReadyForPrompt()
	{
		return ReadyForPrompt?.Invoke();
	}

	private void ExitBlock(int flag)
	{
		PlayerPrefs.SetInt("badflag", 0);
		PlayerPrefs.SetInt("exit_flag", flag);
		PlayerPrefs.SetInt("lastBlockLine", PlayerPrefs.GetInt("line", 0));
		SceneManager.LoadScene("End");
	}
}