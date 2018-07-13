using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IFixation {

	void ShowFixation();

	IEnumerator ProgressFixation();

	void CompleteFixation();
}