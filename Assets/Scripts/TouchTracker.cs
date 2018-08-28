using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchTracker : MonoBehaviour, IDataCollector {

	TrialDelegate trialDelegate;
	Coroutine touchTrackingCoroutine;
	List<Vector2> touches;
	List<Vector2> rawTouches;
	List<string> touchedObjectNames;
	List<float> touchTimes;

	bool trackTouches = false;

	int initialSize = 200;

	public string DataHeaders()
	{
		var firstTouch = "touchX" + DataRecorder.separator + "touchY";
		var firstTouchTime = "touchTime";
		var lastTouch = "lastTchX" + DataRecorder.separator + "lastTchY";
		var lastTouchTime = "lastTchTime";
		string dataHeader = String.Join(DataRecorder.separator, new List<string>{
			firstTouch, firstTouchTime, lastTouch, lastTouchTime});
		Debug.Log(dataHeader);
		return dataHeader;
	}

	public string Data()
	{
		var firstTouch = touches[0].x.ToString() + DataRecorder.separator + touches[0].y.ToString();
		var firstTouchTime = touchTimes[0].ToString() + DataRecorder.separator + touchTimes[0].ToString();
		var lastTouch = touches[touches.Count-1].x.ToString() + DataRecorder.separator + touches[touches.Count-1].y.ToString();
		var lastTouchTime = touchTimes[touchTimes.Count - 1].ToString() + DataRecorder.separator + touchTimes[touchTimes.Count - 1].ToString();
		return String.Join(DataRecorder.separator,
			new List<string>
			{
				firstTouch, firstTouchTime, lastTouch, lastTouchTime
			});
	}

	private void Awake()
	{
		
	}

	// Use this for initialization
	void Start () {
		touches = new List<Vector2>(initialSize);
		rawTouches = new List<Vector2>(initialSize);
		touchedObjectNames = new List<string>(initialSize);
		touchTimes = new List<float>(initialSize);
	}
	
	// Update is called once per frame
	void Update () {

	}

	void OnEnable()
	{
		TrialDelegate.ReadyToPresentStimuli += StartTouchTracking;
		TrialDelegate.ReadyForFeedback += StopTouchTracking;
	}

	private void OnDisable()
	{
		TrialDelegate.ReadyToPresentStimuli -= StartTouchTracking;
		TrialDelegate.ReadyForFeedback -= StopTouchTracking;
	}

	void StartTouchTracking()
	{
		if(trackTouches)
		{
			Debug.Log("Touch tracker is already enabled.");
		}
		else
		{
			Debug.Log("Touch tracker is tracking touch data.");
			trackTouches = true;
			touchTrackingCoroutine = StartCoroutine(touchTracking());
		}
	}

	void StopTouchTracking()
	{
		trackTouches = false;
		if(touchTrackingCoroutine != null)
		{
			StopCoroutine(touchTrackingCoroutine);
		}
		Debug.Log("Number of touches tracked is " + touches.Count);
		Debug.Log("Touch tracker stopped tracking touch data.");
	}

	IEnumerator touchTracking()
	{
		while (true)
		{
			RecordTouchData();
			yield return null;
		}
	}
	
	void RecordTouchData()
	{
		Vector2 position;
		if(Input.touchCount <= 0 && !Input.GetMouseButton(0)) // no touch, no mouse press
		{
			return;
		}
		else if (Input.touchCount > 0) // was it a touch?
		{
			Touch touch = Input.GetTouch(0);
			position = touch.position;
			rawTouches.Add(touch.rawPosition); // this one is touch only.
			Debug.Log("Raw position: " + touch.rawPosition);
		}
		else { // no; then it must be a mouse button press
			position = Input.mousePosition;
			touchTimes.Add(Time.time);
		}
		Debug.Log("Position: " + position);
		touches.Add(position);
		touchTimes.Add(Time.time);
		touchedObjectNames.Add(CheckCollisions(position));
	}

	string CheckCollisions(Vector2 pointerClick)
	{
		string name = "";
		// do the ray casting here
		// source: https://answers.unity.com/questions/1126621/best-way-to-detect-touch-on-a-gameobject.html
		Vector3 touchPosWorld = Camera.main.ScreenToWorldPoint(new Vector3(pointerClick.x, pointerClick.y));
		Vector2 touchPosWorld2D = new Vector2(touchPosWorld.x, touchPosWorld.y);
		RaycastHit2D hitInfo = Physics2D.Raycast(touchPosWorld2D, Camera.main.transform.forward);

		if(hitInfo.collider != null)
		{
			name = hitInfo.transform.gameObject.transform.name;
			name.Replace("(Clone)", "");
			Debug.Log("Touched " + name);
		}
		return name;
	}
}
