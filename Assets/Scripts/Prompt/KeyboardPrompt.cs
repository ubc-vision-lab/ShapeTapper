using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyboardPrompt : AbstractPrompt, IDataCollector {

	[SerializeField] List<KeyCode> keyCodes;
	bool listeningForKeyboard = false;
	KeyCode response;
	SpriteRenderer promptImageRenderer;
	TrialDelegate trialDelegate;

	public override void Prompt()
	{
		Debug.Log("Prompting...");
		promptImageRenderer.enabled = true;
		listeningForKeyboard = true;
	}

	public override void EndPrompt()
	{
		promptImageRenderer.enabled = false;
		listeningForKeyboard = false;
	}

	public string DataHeaders()
	{
		return "KeyPress";
	}

	public string Data()
	{
		return response.ToString();
	}

	private void Awake()
	{
		trialDelegate = FindObjectOfType<TrialDelegate>();
		promptType = "Keyboard Prompt";
		promptImageRenderer = gameObject.GetComponent<SpriteRenderer>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if(listeningForKeyboard)
		{
			foreach(KeyCode keyCode in keyCodes)
			{
				if(Input.GetKeyDown(keyCode))
				{
					PlayerPrefs.SetString("", keyCode.ToString());
					Debug.Log(keyCode.ToString() + "was pressed!");
					EndPrompt();
					trialDelegate.OnReadyForFeedback();
					response = keyCode;
				}
			}
		}
	}

	private void OnEnable()
	{
		TrialDelegate.ReadyForPrompt += Prompt;
		Debug.Log(promptType + " added to Trial.");
	}

	private void OnDisable()
	{
		EndPrompt();
		TrialDelegate.ReadyForPrompt -= Prompt;
		Debug.Log(promptType + " removed from Trial.");
	}
}
