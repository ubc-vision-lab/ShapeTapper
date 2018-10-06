using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbstractPrompt : MonoBehaviour {

	[SerializeField] protected string promptType = "AbstractPrompt";

	/// <summary>
	/// Enable the prompter to accept input
	/// </summary>
	public abstract void Prompt();

	/// <summary>
	/// Disables the prompter. Any input that was made will be saved.
	/// </summary>
	public abstract void EndPrompt();

	void OnEnable()
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
