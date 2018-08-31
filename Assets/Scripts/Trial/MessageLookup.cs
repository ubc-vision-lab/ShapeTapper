using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu()]
public class MessageLookup : ScriptableObject {

	// As per the 
	[SerializeField] List<MessagePair> networkMessages;

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
}
