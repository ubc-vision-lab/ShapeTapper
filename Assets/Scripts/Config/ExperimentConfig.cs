using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using EnnsLab;
using System.IO;
using UnityEngine.SceneManagement;


/* Experiment Config:
 *  Singleton to store trial data, non-persistent but created to avoid reading
 *  the file each trial
*/
public class ExperimentConfig : MonoBehaviour {

	public static ExperimentConfig instance;

	public AssetBundle assetBundle;
	private List<TrialConfig> trialConfigs = new List<TrialConfig>();
	private const string linePrefName = "line";
	private List<TrialConfig> badTrials = new List<TrialConfig>();

	// Use this for Singleton initialization
	void Awake()
	{
		assetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/images");
		if(assetBundle == null)
		{
			// rip. no asset bundle = no experiment
			Debug.Log("No Asset Bundle found.");
		}
		else { Debug.Log("Asset bundle loaded."); }
		if(instance != null)
		{
			if (instance == this)
			{
				Debug.Log("Instance of of ExperimentConfig already exists. Destroying this object.");
			}
			else
			{
				Debug.Log("Instance of of ExperimentConfig already exists. Destroying this object.");
				Destroy(this);
			}
		}
		else
		{
			Debug.Log("Parsing config file...");
			// parse the file
			parseConfig();
			Debug.Log("Parse complete. Saving as singleton instance...");
			instance = this;
			DontDestroyOnLoad(this);
			Debug.Log("ExperimentConfig instantiated.");
		}
	}

	private void OnDestroy()
	{
		assetBundle.Unload(true);
	}

	#region SetupFunctions
	/** parseConfig(configFName) => bool
	 *   Given a filename with extension, parses the configuration file.
	 *   returns true if successful
	 *   TODO: throw exception if malformed config file, or file does not exist.
	 */
	public bool parseConfig(string configFName = null)
	{
		PlayerPrefs.SetString("configName", "sf_01.txt");
		var configFullPathName = Application.persistentDataPath + "/";
		configFullPathName += (configFName == null) ?
			PlayerPrefs.GetString("configName", "sf_01.txt") : // default
			configFName; // when parameter put in
		Debug.Log(PlayerPrefs.GetString("configName", ""));

		try
		{
			StreamReader configStream = new StreamReader(configFullPathName);
			while (!configStream.EndOfStream) // read the whole file
			{
				//TODO: Swap this out for CreateInstance
				trialConfigs.Add(new TrialConfig(configStream.ReadLine()));
			}
			configStream.Close();
		} catch(Exception e) // something's wrong with the file?
		{
			Debug.Log("Error parsing file " + configFullPathName);
			Debug.LogError(e);
			PlayerPrefs.SetInt("exit_flag", 1);
			//SceneManager.LoadScene("End");
			return false;
		}
		return true;
	}

	public void Reset()
	{
		if (instance == null)
		{
			return;
		}
		else
		{
			instance.trialConfigs.Clear();
		}
	}
	#endregion SetupFunctions

	public TrialConfig GetCurrentConfig()
	{
		return trialConfigs[PlayerPrefs.GetInt(linePrefName,0)];
	}

	public bool EndOfBlock()
	{
		var configFileTrialNumber = PlayerPrefs.GetInt(linePrefName, 0);
		if (trialConfigs[configFileTrialNumber + 1] == null)
		{
			return true;
		}
		else
		{
			return trialConfigs[configFileTrialNumber + 1].Config[(int)TrialConfig.ConfigIndex.block] == trialConfigs[configFileTrialNumber].Config[(int)TrialConfig.ConfigIndex.block];
		}
	}

	public bool EndOfExperiment()
	{
		return PlayerPrefs.GetInt(linePrefName, 0) >= trialConfigs.Count;
	}
}
