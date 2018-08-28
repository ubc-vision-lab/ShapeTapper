using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(TrialDelegate))]
public class TrialStateTracker : MonoBehaviour, IDataCollector {

	TrialDelegate trialDelegate;
	List<string> messages;
	MATLABclient mATLABclient; // this name looks bad and you should feel bad :(
	bool isValidTrial;

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
		return "Trial Errors";
	}

	public string Data()
	{
		return lastMATLABState;
	}

	public bool IsValidTrial()
	{
		return isValidTrial;
	}
}
