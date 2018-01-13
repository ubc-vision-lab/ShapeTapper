using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject : MonoBehaviour {

    List<Observer> observers = new List<Observer>();
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Notify()
    {
        foreach(Observer observer in observers)
        {
            observer.OnNotify();
        }
    }

    public void AddObserver(Observer observer)
    {
        observers.Add(observer);
    }

    public void RemoveObserver(Observer observer)
    {
        if(observers.Contains(observer))
        {
            observers.Remove(observer);
        }
    }
}
