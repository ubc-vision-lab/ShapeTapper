using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnnsLab
{
	public class SimultaneousStimulusPresenter : AbstractPresenter
	{

		private List<Stimulus> StimulusEvents;

		event FixationEvent OnPresentation
		{
			add
			{
				throw new NotImplementedException();
			}

			remove
			{
				throw new NotImplementedException();
			}
		}

		event FixationEvent AfterPresentation
		{
			add
			{
				throw new NotImplementedException();
			}

			remove
			{
				throw new NotImplementedException();
			}
		}

		protected override IEnumerator Present()
		{
			throw new NotImplementedException();
		}

		// Use this for initialization
		void Awake()
		{
			// doesn't really do anything here
		}

		private void OnEnable()
		{

		}
	}
}