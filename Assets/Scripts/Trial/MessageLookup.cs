using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class MessageLookup : ScriptableObject {

	// As per the 
	[SerializeField] List<string> networkMessages;
	[SerializeField] List<string> associatedMeanings;

	Dictionary<string, string> messageDictionary;

	public Dictionary<string, string> MessageDictionary
	{
		get
		{
			return messageDictionary;
		}

		set
		{
			messageDictionary = value;
		}
	}

	private void Awake()
	{
		MessageDictionary = networkMessages.Zip(associatedMeanings, (k, v) => new { k, v }).
			ToDictionary(x => x.k, x => x.v);
	}
}
