using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IStimulus {

	IEnumerator Stimulate();

	void Terminate();

	bool CoroutineIsFinished();
}
