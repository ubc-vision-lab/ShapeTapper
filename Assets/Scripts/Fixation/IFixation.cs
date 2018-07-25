using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFixation {

	IEnumerator RunFixation();

	void ShowFixation();

	IEnumerator ProgressFixation();

	void CompleteFixation();
}