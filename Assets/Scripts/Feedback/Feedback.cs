using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class Feedback : MonoBehaviour {

	TrialDelegate trialDelegate;
	bool feedback = true;
	Text feedbackText;
	List<string> networkMessages;
	Coroutine feedbackCoroutine;

	[Range(0,60)] [SerializeField] float feedbackTime;

	private void Awake()
	{
		trialDelegate = FindObjectOfType<TrialDelegate>();
		var trialSetting = ExperimentConfig.instance.GetCurrentConfig().TrialSetting;
		feedback = trialSetting._feedback;
		feedback = true; // TODO: remove this
		feedbackText = gameObject.GetComponent<Text>();
		feedbackText.text = "HELLO VISION LAB";
		feedbackText.enabled = false;
		networkMessages = new List<string>();
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	private void OnEnable()
	{
		if (feedback)
		{
			Debug.Log("Feedback Enabled.");
			TrialDelegate.ReadyForFeedback += GiveFeedback;
			MATLABclient.OnMessageReceived += ParseMessage;
		}
		else Debug.Log("Feedback not enabled.");
	}

	private void OnDisable()
	{
		if (feedback)
		{
			Debug.Log("Disabling feedback");
			TrialDelegate.ReadyForFeedback -= GiveFeedback;
			MATLABclient.OnMessageReceived -= ParseMessage;
		}
	}

	void ParseMessage(string message)
	{
		feedbackText.text += "\n" + message;
		networkMessages.Add(message);
	}

	void GiveFeedback()
	{
		Debug.Log("Running Feedback");
		feedbackCoroutine = StartCoroutine(FeedbackCoroutine());
		Debug.Log("Feedback message: " + feedbackText.text);
		Debug.Log("Finished Feedback");
	}

	IEnumerator FeedbackCoroutine()
	{
		feedbackText.enabled = true;
		yield return new WaitForSecondsRealtime(feedbackTime);
		feedbackText.enabled = false;
		trialDelegate.OnReadyToEndTrial();
	}

	private void OnDestroy()
	{
		if(feedbackCoroutine != null)
		{
			StopCoroutine(feedbackCoroutine);
		}
		feedbackText.enabled = false;
	}
}