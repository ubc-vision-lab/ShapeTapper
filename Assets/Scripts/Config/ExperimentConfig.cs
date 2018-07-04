using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using EnnsLab;
using System.IO;
using UnityEngine.SceneManagement;
using NUnit.Framework;


/* Experiment Config:
 *  Singleton to store trial data, non-persistent but created to avoid reading
 *  the file each trial
*/
public class ExperimentConfig : MonoBehaviour {

	public static ExperimentConfig instance;

	public AssetBundle assetBundle;
	private List<TrialConfig> trialConfigs = new List<TrialConfig>();
	private const string linePrefName = "line";

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
				return;
			}
			else
			{
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
		var configFullPathName = Application.persistentDataPath + "/";
		configFullPathName += (configFName == null) ?
			PlayerPrefs.GetString("configName", "config_spider.txt") : // default
			configFName; // when parameter put in

		try
		{
			StreamReader configStream = new StreamReader(configFullPathName);
			while (!configStream.EndOfStream) // read the whole file
			{
				trialConfigs.Add(new TrialConfig(configStream.ReadLine()));
			}
			configStream.Close();
		} catch(Exception e) // something's wrong with the file?
		{
			Debug.Log("Error parsing file " + configFullPathName);
			Debug.Log("StreamReader returned with the following error: " + e.Message);
			PlayerPrefs.SetInt("exit_flag", 1);
			SceneManager.LoadScene("End");
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

	#region Tests
	[Test]
	public void GetCurrentConfigTest()
	{
		instance = new ExperimentConfig();
		instance.parseConfig("ExperimentConfigTest.txt");
		TrialConfig expected = new TrialConfig("1,4,0,0,0,0,5,,,,,,,1,66.0000,55.0000,1,neu_pos_teddy_bear,234,8.1260,1,0,5,0,0,pos_teddy_bear,0,8.1260,0,0,5,0,0,,144,8.1260,0,0,5,0,1,64.0000,55.0000,,,1");
		PlayerPrefs.SetInt("line", 3);
		Assert.That(instance.GetCurrentConfig() == expected);
	}
	#endregion Tests

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
