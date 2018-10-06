using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KeyboardListener : MonoBehaviour {

	[SerializeField] KeyCode keyCode;
	[SerializeField] int sceneNumber;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if(Input.GetKeyDown(keyCode))
		{
			SceneManager.LoadScene(sceneNumber);
		}
	}
}
