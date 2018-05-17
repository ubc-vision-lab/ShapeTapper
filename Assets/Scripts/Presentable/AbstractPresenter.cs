using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void FixationEvent();

public abstract class AbstractPresenter : MonoBehaviour {

    event FixationEvent OnPresentation; // Anything that needs to be presented simultaneously
    event FixationEvent AfterPresentation; // Something to be done after

    // Runs the behaviour of the fixation
    protected abstract IEnumerator Present();


}