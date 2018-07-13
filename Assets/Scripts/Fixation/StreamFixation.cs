using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class StreamFixation : MonoBehaviour, IFixation
{

	#region private fields
	private string _possibleLetters;
	private string[] charStream;
	private string _targetLetter = "";
	public Text _charStreamText; // jokes this one is public

	private int _targetIdx;
	private int _repeatDistance;
	private float _targetOnsetTime;
	private float _targetRecordedDuration;
	private float _stimulusDuration;
	private float _displayDuration;
	#endregion

	#region Properties
	public string PossibleLetters
	{
		get
		{
			return _possibleLetters;
		}

		set
		{
			_possibleLetters = value;
		}
	}

	public string[] CharStream
	{
		get
		{
			return charStream;
		}

		set
		{
			charStream = value;
		}
	}

	public string TargetLetter
	{
		get
		{
			return _targetLetter;
		}

		set
		{
			_targetLetter = value;
		}
	}

	public Text CharStreamText
	{
		get
		{
			return _charStreamText;
		}

		set
		{
			_charStreamText = value;
		}
	}


	public int TargetIdx
	{
		get
		{
			return _targetIdx;
		}

		set
		{
			_targetIdx = value;
		}
	}

	public float Target_onset_time
	{
		get
		{
			return _targetOnsetTime;
		}

		set
		{
			_targetOnsetTime = value;
		}
	}

	public float TargetRecordedDuration
	{
		get
		{
			return _targetRecordedDuration;
		}

		set
		{
			_targetRecordedDuration = value;
		}
	}

	public float StimulusDuration
	{
		get
		{
			return _stimulusDuration;
		}

		set
		{
			_stimulusDuration = value;
		}
	}

	public float DisplayDuration
	{
		get
		{
			return _displayDuration;
		}

		set
		{
			_displayDuration = value;
		}
	}

	public int RepeatDistance
	{
		get
		{
			return _repeatDistance;
		}

		set
		{
			_repeatDistance = value;
		}
	}
	#endregion Properties

	#region Constructors
	public StreamFixation(string possibleLetters, Vector3 fixation_position, float stimulus_duration, float display_duration)
	{
		CharStreamText = Instantiate(CharStreamText, new Vector3(0,1),
							new Quaternion());
		// CharStreamText.transform.SetParent(renderCanvas.transform, false);
		PossibleLetters = possibleLetters;
		throw new NotImplementedException();
	}
	#endregion

	#region Interface Methods
	public void ShowFixation()
	{

	}

	public IEnumerator ProgressFixation()
	{
		WaitForSecondsRealtime waitStimulusTime = new WaitForSecondsRealtime(StimulusDuration); // We always want this to be this period
		foreach (string character in CharStream)
		{
			CharStreamText.text = character;
			CharStreamText.enabled = true;
			float charStartTime = Time.time;
			if (character.Equals(TargetLetter))
			{
				RecordTargetTime(charStartTime);
			}
			yield return waitStimulusTime;
			CharStreamText.enabled = false;
			float actualDuration = Time.time - charStartTime;
			if (character.Equals(TargetLetter))
			{
				RecordTargetDuration(actualDuration);
			}
			yield return new WaitForSeconds(Math.Max(0,DisplayDuration - actualDuration)); // We're not always sure of how long the presentation is so we want to make up the difference and keep intervals consistent
		}
	}

	public void CompleteFixation()
	{
		throw new NotImplementedException();
	}
	#endregion Interface Methods

	#region Methods
	private void RecordTargetTime(float charStartTime)
	{
		Target_onset_time = charStartTime;
	}

	private void RecordTargetDuration(float actualDuration)
	{
		TargetRecordedDuration = actualDuration;
	}

	private void MakeCharStream(int repeat_distance)
	{
		PickLetter();
		CharStream = new string[TargetIdx + 15];
		Queue<string> previous_chars = new Queue<string>(repeat_distance);
		for (int i = 0; i < CharStream.Length; i++)
		{
			string tmp;
			if (i == TargetIdx)
			{
				tmp = TargetLetter;
			}
			else
			{
				tmp = UnityEngine.Random.Range(0, 10).ToString(); // all the single digit numbers
				while (previous_chars.Contains(tmp)) // until there's no string that's the same as the first two
				{
					tmp = UnityEngine.Random.Range(0, 10).ToString();
				}
			}
			if (previous_chars.Count >= repeat_distance)
			{
				previous_chars.Dequeue();
			}
			previous_chars.Enqueue(tmp); // last two characters
			CharStream[i] = tmp;
		}
	}

	private void PickLetter()
	{
		TargetIdx = UnityEngine.Random.Range(5, 11);
		TargetLetter = PossibleLetters[UnityEngine.Random.Range(0, PossibleLetters.Length)].ToString();
	}
	#endregion Methods

	#region Unity Event Functions
	private void Awake()
	{
		
	}

	private void OnEnable()
	{
		
	}
	#endregion

	#region Tests
	[Test]
	public void NoRepetitionsOfPreviousTwoChars()
	{
		int previousTwo = 2;
		// TODO: Make the factory methods
		StreamFixation streamFixation = new StreamFixation("KNVXZ",Vector3.zero,0.07f,0.1f);
		streamFixation.MakeCharStream(previousTwo);
		Queue<string> checkQueue = new Queue<string>();
		foreach (string character in streamFixation.CharStream)
		{
			Assert.That(!checkQueue.Contains(character));
			if(checkQueue.Count >= previousTwo)
			{
				checkQueue.Dequeue();
			}
			checkQueue.Enqueue(character);
		}
	}
	#endregion Tests
}