using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrialDelegate))]
public class TrialStateTracker : MonoBehaviour, IDataCollector {

	List<string> messages = new List<string>();
	MATLABclient mATLABclient; // this name looks bad XD
	bool isValidTrial = true;

	[SerializeField] MessageLookup mATLABMessageDictionary;

	string lastMATLABState;

	void OnEnable()
	{
		lastMATLABState = "";
		MATLABclient.OnMessageReceived += ParseMessage;
	}

	void OnDisable()
	{
		MATLABclient.OnMessageReceived -= ParseMessage;
	}

	void ParseMessage(string message)
	{
		messages.Add(message);
		lastMATLABState = mATLABMessageDictionary.MessageDictionary[message];
	}

	// IDataCollector methods
	public string DataHeaders()
	{
		return "trial_errors";
	}

	public string Data()
	{
		return lastMATLABState;
	}

	public bool IsValidTrial()
	{
		return isValidTrial;
	}

	public void SetBadTrial()
	{
		isValidTrial = false;
	}
}
