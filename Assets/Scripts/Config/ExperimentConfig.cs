using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using EnnsLab;
using System.IO;
using UnityEngine.SceneManagement; // use this for invalid config files?

/// <summary>
/// Experiment config parses the configuration file and stores the configuration data.
/// It is also responsible for holding onto any trials that have gone badly.
/// All stored data is temporary, though the current trial and block are saved to disk.
/// </summary>
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

	[SerializeField] string endOfBlockScene;
	[SerializeField] string nextTrialScene;

	/// <summary>
	/// Instantiate ExperimentConfig if it doesn't already exist.
	/// Parse all Configuration files while you're at it.
	/// </summary>
	void Awake()
	{
		if (instance != null)
		{
			if (instance != this)
			{
				Debug.Log("Instance of of ExperimentConfig already exists. Destroying this object.");
				Destroy(this);
			}
		}
		else // there is no instance. Setup time!
		{
			assetBundle = AssetBundle.LoadFromFile(Application.persistentDataPath + "/images");
			if (assetBundle == null)
			{
				// rip. no asset bundle = no experiment
				Debug.Log("No Asset Bundle found.");
			}
			else { Debug.Log("Asset bundle is loaded."); }

			Debug.Log("Parsing config file...");
			// parse the file
			parseConfig();
			Debug.Log("Parse complete. Saving as singleton instance...");
			instance = this;
			DontDestroyOnLoad(this);
			Debug.Log("ExperimentConfig instantiated.");
		}
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

		Stream configStream = null;
		try
		{
			configStream = new FileStream(configFullPathName,FileMode.Open);
			using (StreamReader reader = new StreamReader(configStream))
			{
				configStream = null;
				while (!reader.EndOfStream)  // read the whole file
				{
					//TODO: Swap this out for CreateInstance
					trialConfigs.Add(new TrialConfig(reader.ReadLine()));
				}
			}
			PlayerPrefs.SetInt("exit_flag", 0);
			
		} catch(Exception e) // something's wrong with the file?
		{
			Debug.Log("Error parsing file " + configFullPathName);
			Debug.LogError(e);
			PlayerPrefs.SetInt("exit_flag", 1);
			//SceneManager.LoadScene("End");
			return false;
		} finally
		{
			if(configStream != null)
			{
				configStream.Close();
				configStream.Dispose();
			}
		}
		return true;
	}

	/// <summary>
	/// Clear all configurations, including bad trials
	/// </summary>
	public void Reset()
	{
		if (instance == null)
		{
			return;
		}
		else
		{
			instance.trialConfigs.Clear();
			instance.badTrials.Clear();
		}
	}
	#endregion SetupFunctions

	/// <summary>
	/// Gets the current configuration file.
	/// A random trial with a bad result will be selected if all trials in a block
	/// have been run at least once.
	/// </summary>
	/// <returns>
	/// The trial to be run.
	/// </returns>
	public TrialConfig GetCurrentConfig()
	{
		if(AllTrialsInBlockRunOnce() && badTrials.Count > 0)
		{
			var currentTrial = badTrials[UnityEngine.Random.Range(0, badTrials.Count - 1)];
			badTrials.Remove(currentTrial);
			return currentTrial;
		}
		return trialConfigs[PlayerPrefs.GetInt(linePrefName,0)];
	}

	/// <summary>
	/// Goes to the next trial. If it's 
	/// </summary>
	public void AdvanceTrial()
	{
		var trialStateTracker = FindObjectOfType<TrialStateTracker>();

		if (trialStateTracker != null)
		// if there's no trial states, then just assume that we advance
		{
			// if it's a bad trial add it to the pool
			if (!trialStateTracker.IsValidTrial())
			{
				ExperimentConfig.instance.AddToBadTrials();
			}
		}

		if(!AllTrialsInBlockRunOnce()) // check whether we need to increment the line counter
		{
			// we haven't even finished the block yet; go to the next trial
			PlayerPrefs.SetInt("line", PlayerPrefs.GetInt("line", 0) + 1);
			SceneManager.LoadScene(nextTrialScene);
		}
		else if (!(badTrials.Count > 0)) // if there aren't any bad trials left
		{
			PlayerPrefs.SetInt("line", PlayerPrefs.GetInt("line", 0) + 1);
			// load the next scene?
			// the number of blocks left should also increase
			PlayerPrefs.SetInt("block", GetCurrentConfig().TrialSetting._block_no);
			SceneManager.LoadScene(endOfBlockScene);
		}
	}

	public bool AllTrialsInBlockRunOnce()
	{
		var configFileTrialNumber = PlayerPrefs.GetInt(linePrefName, 0);
		if (trialConfigs[configFileTrialNumber + 1] == null) // end of experiment
		{
			return true; 
		}
		else
		{
			// return whether the block numbers are equal
			return trialConfigs[configFileTrialNumber + 1].Config[(int)TrialConfig.ConfigIndex.block]
				!= trialConfigs[configFileTrialNumber].Config[(int)TrialConfig.ConfigIndex.block];
		}
	}

	//
	public bool EndOfExperiment()
	{
		return PlayerPrefs.GetInt(linePrefName, 0) >= trialConfigs.Count;
	}

	public void AddToBadTrials()
	{
		badTrials.Add(GetCurrentConfig());
	}

	public void AddToBadTrials(TrialConfig trialConfig)
	{
		badTrials.Add(trialConfig);
	}
}
