﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimultaneousPresenter : AbstractPresenter
{
	public override IEnumerator Present()
	{
		foreach (IStimulus stimulus in ToBePresented)
		{
			stimulusCoroutines.Add(StartCoroutine(stimulus.Stimulate()));
		}
		throw new NotImplementedException();
	}

	// Unity setup functions are done in AbstractPresenter
}