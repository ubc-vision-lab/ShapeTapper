using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnnsLab
{
	public class TrialSetting : ScriptableObject
	{
		public int _block_no;
		public int _trial_no;
		public bool _feedback;
		public bool _practice = false;
		public float _block_percentage = 0;
		public bool _block_feedback = false;
		public float _time_to_respond;
		public string _too_slow_img = "";
		public int _exp_mode;
		public int _ask_for_target;
		public float _stimulus_onset = 0f;
		public bool _loop_trial = false;

		/*
		 * Given string of information, creates a TrialSetting. String must be
		 * in the following order:
		 *   block number
		 *   trial number
		 *   feedback
		 *   practice
		 *   block percentage
		 *   block feedback
		 *   time to respond
		 *   image when user doesn't respond in time
		 *   
		 */
		public TrialSetting(List<string> info)
		{
			Debug.Assert(info.Count == 10);
			int.TryParse(info[0], out _block_no);
			int.TryParse(info[1], out _trial_no);
			_feedback = int.Parse(info[2]) == 1;
			_practice = int.Parse(info[3]) == 1;
			float.TryParse(info[4], out _block_percentage);
			_block_feedback = int.Parse(info[5]) == 1;
			float.TryParse(info[6], out _time_to_respond);
			_too_slow_img = info[7];
			int.TryParse(info[8], out _exp_mode);
			int.TryParse(info[9], out _ask_for_target);
			float.TryParse(info[10], out _stimulus_onset);
            _loop_trial = int.Parse(info[11]) == 1;
		}

		public TrialSetting(int block_no, int trial_no, bool feedback,
			bool practice, float block_percentage, bool block_feedback,
			float time_to_respond, string too_slow_img, int exp_mode,
			int ask_for_target, float stimulus_onset, bool loop_trial)
		{
			_block_no = block_no;
			_trial_no = trial_no;
			_feedback = feedback;
			_practice = practice;
			_block_percentage = block_percentage;
			_block_feedback = block_feedback;
			_time_to_respond = time_to_respond;
			_too_slow_img = too_slow_img;
			_exp_mode = exp_mode;
			_ask_for_target = ask_for_target;
            _stimulus_onset = stimulus_onset;
            _loop_trial = loop_trial;
		}
	}

	
}