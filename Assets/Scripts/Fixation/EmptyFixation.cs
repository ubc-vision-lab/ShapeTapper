using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnnsLab
{
	public class NoFixation : AbstractPresenter {

		// Use this for initialization
		void Start () {
		
		}
	
		// Update is called once per frame
		void Update () {
		
		}

		public override IEnumerator Present()
		{
			yield return new WaitForSeconds(0f);
		}
	}
}

