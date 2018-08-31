using EnnsLab;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrialDelegate : MonoBehaviour {

	public delegate void TrialEvent();

	public static event TrialEvent ReadyToStartTrial;
	public static event TrialEvent ReadyToPresentStimuli;
	public static event TrialEvent ReadyToPresentConditionalStimuli;
	public static event TrialEvent ReadyForPrompt;
	public static event TrialEvent ReadyForFeedback;
	public static event TrialEvent ReadyForEndTrial;

	// Setup checks
	private bool fixationReady = false;
	private bool stimulusReady = false;
	private bool promptReady = false;

	// trial tracking booleans
	private bool triggerReady = false;
	private bool trialStarted = false;

	AbstractPresenter fixation;
	AbstractPrompt prompt;
	TrialStateTracker stateTracker;

	// TODO: Needs more abstraction
	[SerializeField] DataRecorder dataRecorder;
	[SerializeField] MessagePair RestartMessage;
	[SerializeField] MessagePair FixationMessage;

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
	void Start()
	{
		// if there is a PointerTrigger wait for it to be enabled
		var pointerTrigger = FindObjectOfType<PointerTrigger>();
		if (pointerTrigger == null || !pointerTrigger.enabled)
		{
			Debug.Log("Pointer Trigger not enabled or was not found.");
			triggerReady = true;
		}
		else
		{
			Debug.Log("Pointer Trigger found. Waiting for trigger to start.");
			triggerReady = false;
		}
	}

	void Update()
	{
		if(triggerReady && fixationReady && stimulusReady && promptReady && !trialStarted)
		{
			OnReadyToStartTrial();
		}
	}

	private void OnDestroy()
	{
		ReadyToStartTrial = null;
		ReadyToPresentStimuli = null;
		ReadyToPresentConditionalStimuli = null;
		ReadyForPrompt = null;
		ReadyForFeedback = null;
		ReadyForEndTrial = null;
	}

	public void AbortTrial(string message)
	{

	}

	public void OnReadyToStartTrial()
	{
		if (ReadyToStartTrial != null)
		{
			ReadyToStartTrial.Invoke();
		}
	}

	public void OnReadyToPresentStimuli()
	{
		if (ReadyToPresentStimuli == null)
		{
			Debug.Log("ReadyToPresentStimuli is empty.");
		}
		else
		{
			Debug.Log("ReadyToPresentStimuli.Invoke");
			ReadyToPresentStimuli.Invoke();
		}
	}

	public void OnReadyToPresentConditionalStimuli()
	{
		if (ReadyToPresentConditionalStimuli == null)
		{
		}
		else ReadyToPresentConditionalStimuli.Invoke();
	}

	public void OnReadyForPrompt()
	{
		if (ReadyForPrompt == null)
		{
		}
		else ReadyForPrompt.Invoke();
	}

	public void OnReadyForFeedback()
	{
		if (ReadyForFeedback == null)
		{

		}
		else ReadyForFeedback.Invoke();
		Debug.Log("ReadyForFeedback");
	}
	
	public void OnReadyToEndTrial()
	{
		if (ReadyForEndTrial == null)
		{

		}
		else ReadyForEndTrial.Invoke();
		Debug.Log("ReadyToEndTrial");
		ExperimentConfig.instance.AdvanceTrial();
	}

	private void ExitBlock(int flag)
	{
		PlayerPrefs.SetInt("badflag", 0);
		PlayerPrefs.SetInt("exit_flag", flag);
		PlayerPrefs.SetInt("lastBlockLine", PlayerPrefs.GetInt("line", 0));
		SceneManager.LoadScene("End");
	}

	private void EnableNetworkListener()
	{
	}

	private void OnMessageReceivedDuringTrial(string message)
	{
		if(message == RestartMessage.message)
		{
			// this was a bad trial!
			OnReadyForFeedback();
		}
	}

	#region Trial Ready functions
	public bool FixationReady()
	{
		return fixationReady = true;
	}

	public bool StimulusReady()
	{
		return stimulusReady = true;
	}

	public bool PromptReady()
	{
		return promptReady = true;
	}

	public bool TriggerReady()
	{
		return triggerReady = true;
	}
	#endregion
}