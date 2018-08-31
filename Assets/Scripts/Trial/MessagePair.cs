using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class MessagePair : ScriptableObject {

	[SerializeField] public string message;
	[SerializeField] public string meaning;
}