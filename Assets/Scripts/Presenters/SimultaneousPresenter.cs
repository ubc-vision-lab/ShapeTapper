using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnnsLab
{
	public class StimulusPresenter : AbstractPresenter
	{

		public override IEnumerator Present()
		{
			foreach (IStimulus stimulus in ToBePresented)
			{
				yield return stimulus.Stimulate();
			}
			throw new NotImplementedException();
		}

		// Use this for initialization
		void Awake()
		{
			// check the config info to determine presentation mode
			throw new NotImplementedException();
		}

		protected override void OnEnable()
		{
			base.OnEnable();
		}

		protected override void OnDisable()
		{
			base.OnDisable();
		}
	}
}