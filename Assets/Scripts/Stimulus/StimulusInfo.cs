using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static EnnsLab.TrialConfig;

namespace EnnsLab
{

	public class StimulusInfo : ScriptableObject
	{

		// stuff all stimuli will have
		private List<string> stimulusNames; // names for all images [including all dynamic mask images]
		private bool is_target;
		private Vector2 position;
		private float rotation;
		private float scaling; // this is a percentage of the vertical space of the camera
		private float display_interval; // how long the stimulus will be on for, not used for dynamic mask
		private float interstimulus_time; // how long an image should be off before the next stimulus begins

		private string maskName; // the four-dot mask
		private float maskLingerTime; // how long the mask sticks around for after the stimulus stop presenting themselves

		// dynamic mask stuff
		private bool is_dyn_mask;
		private float dyn_mask_time;

		#region Properties
		public List<string> StimulusNames
		{
			get
			{
				return stimulusNames;
			}

			set
			{
				stimulusNames = value;
			}
		}

		public bool Is_target
		{
			get
			{
				return is_target;
			}

			set
			{
				is_target = value;
			}
		}

		public Vector2 Position
		{
			get
			{
				return position;
			}

			set
			{
				position = value;
			}
		}

		public float Rotation
		{
			get
			{
				return rotation;
			}

			set
			{
				rotation = value;
			}
		}

		public float Scaling
		{
			get
			{
				return scaling;
			}

			set
			{
				scaling = value;
			}
		}

		public float Display_interval
		{
			get
			{
				return display_interval;
			}

			set
			{
				display_interval = value;
			}
		}

		public float Interstimulus_time
		{
			get
			{
				return interstimulus_time;
			}

			set
			{
				interstimulus_time = value;
			}
		}

		public bool Is_dyn_mask
		{
			get
			{
				return is_dyn_mask;
			}

			set
			{
				is_dyn_mask = value;
			}
		}

		public float Dyn_mask_time
		{
			get
			{
				return dyn_mask_time;
			}

			set
			{
				dyn_mask_time = value;
			}
		}

		public string MaskName
		{
			get
			{
				return maskName;
			}

			set
			{
				maskName = value;
			}
		}

		public float MaskLingerTime
		{
			get
			{
				return maskLingerTime;
			}

			set
			{
				maskLingerTime = value;
			}
		}
		#endregion Properties

		public StimulusInfo(List<string> stimulusNames, bool is_target,
			Vector2 position, float rotation, float scaling,
			float display_interval, float interstimulus_time,
			bool is_dyn_mask, float dyn_mask_time, string maskName = "",
			float maskLingerTime = 0f)
		{
			StimulusNames = stimulusNames;
			Is_target = is_target;
			Position = position;
			Rotation = rotation;
			Scaling = scaling;
			Display_interval = display_interval;
			Interstimulus_time = interstimulus_time;
			Is_dyn_mask = is_dyn_mask;
			Dyn_mask_time = dyn_mask_time;
			MaskName = maskName;
			MaskLingerTime = maskLingerTime;
		}
	}
}