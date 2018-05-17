using EnnsLab;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TrialDelegate : MonoBehaviour {

	public AbstractPresenter fixation;
	public AbstractPresenter stimulus;
	public AbstractPrompt prompt;

	#region Fields
	private AbstractPresenter _trialFixation;
	private AbstractPresenter _stimulusPresenter;
	private string _assetPath;
	private AssetBundle _shapeAssetBundle;
	private TrialConfig currTrialConfig;
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
		// TODO: switch this to use some sort of persistent asset bundle management
		AssetPath = Application.persistentDataPath + "/images";
		ShapeAssetBundle = AssetBundle.LoadFromFile(AssetPath);
		if (ExperimentConfig.instance.EndOfExperiment())
		{
			ExitBlock(9);
		}
		else if (ExperimentConfig.instance.EndOfBlock())
		{
			ExitBlock(0);
		}
		else currTrialConfig = ExperimentConfig.instance.GetCurrentConfig();

		// set up the presentables

		// at the end somewhere here, set up the delegates
		// then present the fixation
	}

	// Update is called once per frame
	void Update () {
		
	}

	private void ExitBlock(int flag)
	{
		PlayerPrefs.SetInt("badflag", 0);
		PlayerPrefs.SetInt("exit_flag", flag);
		PlayerPrefs.SetInt("lastBlockLine", PlayerPrefs.GetInt("line", 0));
		SceneManager.LoadScene("End");
	}
}