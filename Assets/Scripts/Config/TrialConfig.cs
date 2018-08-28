using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace EnnsLab
{
	/* The TrialConfig for ShapeSpider.
	 * All column headers are saved as an enum ConfigIndex below.
	 */
	public class TrialConfig
	{
		private List<string> config;
		private TrialSetting trialSetting;
		private List<StimulusInfo> stimulusData;

		// this stores all the stimulus info columns
		// each row is: [image, target, xpos, ypos, rotation, scaling, image_display_time, interimage_period, dyn_mask, dyn_mask_time]
		// switch this into a dictionary??
		private int[,] stimuliIdcs = // these are the indices... not the actual values
			{
				{
					(int)ConfigIndex.e1_image, // name of image, not used if event is dynamic mask
					(int)ConfigIndex.e1_is_target,
					(int)ConfigIndex.e1_x_pos,
					(int)ConfigIndex.e1_y_pos,
					(int)ConfigIndex.e1_rotation,
					(int)ConfigIndex.e1_scaling, // scale diagonal of image to this percent of screen vertical size
					(int)ConfigIndex.e1_on_time, // display interval
					(int)ConfigIndex.e1_time_to_e2, // time before onset of next event
					(int)ConfigIndex.e1_is_dyn_mask,
					(int)ConfigIndex.e1_dyn_mask_time // how long each mask is displayed
				},
				{
					(int)ConfigIndex.e2_image,
					(int)ConfigIndex.e2_is_target,
					(int)ConfigIndex.e2_x_pos,
					(int)ConfigIndex.e2_y_pos,
					(int)ConfigIndex.e2_rotation,
					(int)ConfigIndex.e2_scaling,
					(int)ConfigIndex.e2_on_time, // display interval
					(int)ConfigIndex.e2_time_to_e3,
					(int)ConfigIndex.e2_is_dyn_mask,
					(int)ConfigIndex.e2_dyn_mask_time
				},
				{
					(int)ConfigIndex.e3_image,
					(int)ConfigIndex.e3_is_target,
					(int)ConfigIndex.e3_x_pos,
					(int)ConfigIndex.e3_y_pos,
					(int)ConfigIndex.e3_rotation,
					(int)ConfigIndex.e3_scaling,
					(int)ConfigIndex.e3_on_time, // display interval
					-1, // last event doesn't have interstimulus time
					(int)ConfigIndex.e3_is_dyn_mask,
					(int)ConfigIndex.e3_dyn_mask_time
				}
			};

		internal string TrialNumber()
		{
			return Config[(int)ConfigIndex.trial];
		}

		#region Properties
		public List<string> Config
		{
			get
			{
				return config;
			}

			set
			{
				config = value;
			}
		}

		public TrialSetting TrialSetting
		{
			get
			{
				return trialSetting;
			}
			set
			{
				trialSetting = value;
			}
		}

		public List<StimulusInfo> StimulusData
		{
			get
			{
				return stimulusData;
			}
			set
			{
				stimulusData = value;
			}
		}
		#endregion Properties

		public enum ConfigIndex : int
		{
			block, trial, feedback, practice,
			block_percentage, block_feedback, time_to_respond,
			too_slow_image,
			dyn_mask1, dyn_mask2, dyn_mask3, dyn_mask4, dyn_mask5,
			loop_trial,
			e1_x_pos, e1_y_pos,
			stimulus_onset,
			e1_image, e1_rotation, e1_scaling, e1_is_target, e1_is_dyn_mask,
				e1_on_time, e1_dyn_mask_time, e1_time_to_e2,
			e2_image, e2_rotation, e2_scaling, e2_is_target, e2_is_dyn_mask,
				e2_on_time, e2_dyn_mask_time, e2_time_to_e3,
			e3_image, e3_rotation, e3_scaling, e3_is_target, e3_is_dyn_mask,
				e3_on_time, e3_dyn_mask_time,
			experiment_mode,
			e2_x_pos, e2_y_pos,
			e3_x_pos, e3_y_pos,
			ask_for_target,
			mask_name, mask_linger_time
		}

		// legacy code: kept for compatibility purposes
		public TrialConfig(string config)
		{
			Config = new List<string>(config.Split(','));

			// fetch the experiment-specific parameters info a data structure
			// TODO: Add a couple more columns to this.
			List<string> expInfoList = Config.GetRange((int)ConfigIndex.block, 8);
			expInfoList.Add(Config[(int)ConfigIndex.experiment_mode]);
			expInfoList.Add(Config[(int)ConfigIndex.ask_for_target]);
			TrialSetting = ParseTrialSetting();
			// put all the stimulus into an array
			StimulusData = ParseAllStimuli();
		}

		private TrialSetting ParseTrialSetting()
		{
			return new TrialSetting(
				int.Parse(config[(int)ConfigIndex.block]),
				int.Parse(config[(int)ConfigIndex.trial]),
				int.Parse(config[(int)ConfigIndex.feedback]) == 1,
				int.Parse(config[(int)ConfigIndex.practice]) == 1,
				float.Parse(config[(int)ConfigIndex.block_percentage]),
				int.Parse(config[(int)ConfigIndex.block_feedback]) == 1,
				float.Parse(config[(int)ConfigIndex.time_to_respond]),
				config[(int)ConfigIndex.too_slow_image],
				int.Parse(config[(int)ConfigIndex.experiment_mode]),
				int.Parse(config[(int)ConfigIndex.ask_for_target]),
				float.Parse(config[(int)ConfigIndex.stimulus_onset]),
				int.Parse(config[(int)ConfigIndex.loop_trial])==1
				);
		}

		/*
		 * Using the pre-defined stimulus array in this class, fetch and parse the correct columns to create StimulusInfo objects.
		 * These can be passed into a StimulusFactory object to create stimulus.
		 */
		public List<StimulusInfo> ParseAllStimuli()
		{
			List<StimulusInfo> events = new List<StimulusInfo>();

			for (int i = 0; i < stimuliIdcs.GetLength(0); i++)
			{
				// each row is: [image, target, xpos, ypos, rotation, scaling,
				//			image_display_time, interimage_period, dyn_mask,
				//			dyn_mask_time]

				// Check if there's anything for this event
				List<string> eventNames = new List<string>
				{
					config[stimuliIdcs[i, 0]] // default to regular event
				};
				eventNames.RemoveAll(p => string.IsNullOrEmpty(p)); // remove empty entries
				bool is_dyn_mask = int.Parse(config[stimuliIdcs[i, 8]]) >= 1;

				if(!is_dyn_mask && eventNames.Count == 0) // no event
				{
					continue;
				}
				else if(is_dyn_mask)
				{
					eventNames.Clear();
					eventNames.AddRange(config.GetRange((int)ConfigIndex.dyn_mask1, 5)); // default to dynamic mask
					eventNames.RemoveAll(p => string.IsNullOrEmpty(p)); // remove empty entries
					if(eventNames.Count == 0)
					{
						// wait also nothing?!
						continue;
					}
				}
				else // it should be something...
				{
					bool is_target = int.Parse(config[stimuliIdcs[i, 1]]) == 1;
					Vector2 position = new Vector2(float.Parse(config[stimuliIdcs[i, 2]]),
												float.Parse(config[stimuliIdcs[i, 3]]));
					float rotation = float.Parse(config[stimuliIdcs[i, 4]]);
					float scaling = float.Parse(config[stimuliIdcs[i, 5]]);
					float display_interval = float.Parse(config[stimuliIdcs[i, 6]]);
					float interstimulus_time =
						(stimuliIdcs[i, 7] != -1) ? float.Parse(config[stimuliIdcs[i, 7]]) : 0f;

					float dyn_mask_time = float.Parse(config[stimuliIdcs[i, 9]]);

					string mask_name = config[(int)ConfigIndex.mask_name];

					if (mask_name != "")
					{
						events.Add(new StimulusInfo(eventNames, is_target, position, rotation, scaling, display_interval, interstimulus_time, is_dyn_mask, dyn_mask_time, mask_name,float.Parse(config[(int)ConfigIndex.mask_linger_time])));
					} else
					{
						events.Add(new StimulusInfo(eventNames, is_target, position, rotation, scaling, display_interval, interstimulus_time, is_dyn_mask, dyn_mask_time));
					}
				}
			}
			return events;
		}
		public string GetTargetName()
		{
			if(stimulusData.Count <= 0)
			{
				return null;
			}
			foreach(StimulusInfo stimInfo in stimulusData)
			{
				if (stimInfo.Is_target)
				{
					return stimInfo.StimulusNames[0];
				}
			}
			return "";
		}

		#region Legacy Code
		// Legacy code, to be deprecated
		//public list<gameobject> loadstimuli(assetbundle assets)
		//{
		//	list<gameobject> stimuli = new list<gameobject>();
		//	string event1_name = config[(int)configindex.e1_image];
		//	string event2_name = config[(int)configindex.e2_image];
		//	string event3_name = config[(int)configindex.e3_image];
		//	if (event1_name != "")
		//	{
		//		gameobject stimulus1 = instantiate<gameobject>(assets.loadasset<gameobject>(event1_name));
		//		stimulus1.getcomponent<renderer>().enabled = false;
		//		stimuli.add(scalestimulus(stimulus1,
		//			float.parse(config[(int)configindex.e1_rotation]),
		//			float.parse(config[(int)configindex.e1_scaling]),
		//			float.parse(config[(int)configindex.e1_x_pos]),
		//			float.parse(config[(int)configindex.e1_y_pos])));
		//	}
		//	if (event2_name != "")
		//	{
		//		gameobject stimulus2 = instantiate<gameobject>(assets.loadasset<gameobject>(event2_name));
		//		stimulus2.getcomponent<renderer>().enabled = false;
		//		stimuli.add(scalestimulus(stimulus2,
		//			float.parse(config[(int)configindex.e2_rotation]),
		//			float.parse(config[(int)configindex.e2_scaling]),
		//			float.parse(config[(int)configindex.e2_x_pos]),
		//			float.parse(config[(int)configindex.e2_y_pos])));
		//	}
		//	if (event3_name != "")
		//	{
		//		gameobject stimulus3 = instantiate<gameobject>(assets.loadasset<gameobject>(event3_name));
		//		stimulus3.getcomponent<renderer>().enabled = false;
		//		stimuli.add(scalestimulus(stimulus3,
		//			float.parse(config[(int)configindex.e3_rotation]),
		//			float.parse(config[(int)configindex.e3_scaling]),
		//			float.parse(config[(int)configindex.e3_x_pos]),
		//			float.parse(config[(int)configindex.e3_y_pos])));
		//	}
		//	return stimuli;
		//}

		//// legacy code, to be deprecated
		//public gameobject scalestimulus(gameobject stimulus, float rotation, float scaling, float x_pos, float y_pos)
		//{
		//	renderer stimulusrenderer = stimulus.getcomponent<renderer>();
		//	stimulusrenderer.enabled = false;
		//	float screenheight = 2f * camera.main.orthographicsize;
		//	float screenwidth = screenheight * camera.main.aspect;
		//	float newimagediag = screenheight * scaling / 100;
		//	//float maximagewidth = screenwidth * scale / 100;

		//	// resize the stimulus to the screen
		//	vector2 img_dim = new vector2(stimulusrenderer.bounds.size.x, stimulusrenderer.bounds.size.y);
		//	float diag = img_dim.magnitude;
		//	float scalefactor = diag / newimagediag;
		//	stimulus.transform.localscale = new vector3(1 / scalefactor, 1 / scalefactor, 1);

		//	float pixelorthoratio = screen.height / 2f / camera.main.orthographicsize;
		//	float border = 3 * screen.dpi / pixelorthoratio / 8; // 3/8ths of an inch on the edges
		//	float margin_x = screenwidth - newimagediag - border;
		//	float margin_y = screenheight - newimagediag - border;
		//	stimulus.transform.position = new vector3(x_pos / 100f * margin_x - (margin_x / 2), y_pos / 100f * margin_y - (margin_y / 2));

		//	stimulus.transform.eulerangles = new vector3(0, 0, rotation);
		//	return stimulus;
		//}
		#endregion Legacy Code
	}
}