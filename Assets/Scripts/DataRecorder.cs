using EnnsLab;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Amalgamates all DataCollectors to write their data into a file.
/// </summary>
public class DataRecorder : MonoBehaviour {

	// set two modes: save per trial and save to playerprefs
	// why are we using player prefs? historical
	TrialConfig trialConfig;
	string fullSubjDir;
	string fileName;
	string fullFileName;

	// DataCollectors
	List<IDataCollector> dataCollectors;
	StreamWriter streamWriter;

	[SerializeField] string dataDir;
	string fullDataDir;
	[SerializeField] public static string separator = "\t";
	// Things that we save on Unity's side:
	/**
	 * trial #,
	 * trial "goodness" (error)
	 * the target (letter, since objects are in the config file)
	 * the position of said target
	 * the picked letter or position
	 * the time the letter/image appeared
	 * the time of the first touch
	 * the touchpoint
	 * last touch time
	 * last touchpoint
	 */


	private void SaveTrialData()
	{
		if(!File.Exists(fullFileName))
		{
			SetupFile();
		}
		streamWriter.Write(trialConfig.TrialNumber() + separator);
		foreach (IDataCollector dataCollector in dataCollectors)
		{
			AddData(dataCollector);
		}
		streamWriter.WriteLine("");
	}

	private void SetupFile()
	{
		fullDataDir = Path.Combine(Application.persistentDataPath, dataDir);
		fullSubjDir = Path.Combine(fullDataDir, PlayerPrefs.GetString("UserID", ""));
		fileName = string.Join("_", PlayerPrefs.GetString("UserID", ""), trialConfig.TrialSetting._block_no);
		if (!Directory.Exists(fullDataDir))
		{
			Directory.CreateDirectory(fullDataDir);
		}
		if (!Directory.Exists(fullSubjDir))
		{
			Directory.CreateDirectory(fullSubjDir);
		}

		fullFileName = Path.Combine(fullSubjDir, fileName + ".txt");
		Debug.Log(fullFileName);
		if(!File.Exists(fullFileName))
		{
			// create the file with the headers
			streamWriter = new StreamWriter(fullFileName);
			streamWriter.Write("trial#" + separator);

			// this is gonna be weird...
			foreach (IDataCollector dataCollector in dataCollectors)
			{
				AddHeader(dataCollector);
			}
			streamWriter.Write("\n");
		}
		else
		{
			// otherwise, just open the file
			streamWriter = new StreamWriter(fullFileName, true);
		}
	}

	private void AddHeader(IDataCollector recorder)
	{
		if(recorder != null)
		{
			streamWriter.Write(recorder.DataHeaders() + separator);
		}
	}

	private void AddData(IDataCollector recorder)
	{
		if(recorder != null)
		{
			streamWriter.Write(recorder.Data() + separator);
		}
		
	}

	private void OnEnable()
	{
		TrialDelegate.ReadyForFeedback += SaveTrialData;
	}

	private void OnDisable()
	{
		TrialDelegate.ReadyForFeedback -= SaveTrialData;
		streamWriter?.Close();
		streamWriter?.Dispose();
	}

	private void Awake()
	{
		trialConfig = FindObjectOfType<ExperimentConfig>().GetCurrentConfig();

		// DataCollectors
		dataCollectors = new List<IDataCollector>();
		// order matters!
		dataCollectors.Add(FindObjectOfType<TrialStateTracker>());
		dataCollectors.Add(FindObjectOfType<KeyboardPrompt>());
		dataCollectors.Add(FindObjectOfType<TouchTracker>());
		SetupFile();
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
