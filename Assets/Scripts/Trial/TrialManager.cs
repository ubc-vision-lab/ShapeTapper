using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace EnnsLab
{
	public partial class TrialManager : Observer
	{

		// Configuration variables
		private string assetPath;
		public AssetBundle imageAssets;
		private string experimentConfigFilename;
		private List<TrialConfig> experimentConfig;
		private TrialConfig current_trial_config;
		private List<string> badTrials;
		private List<GameObject> trialEvents;
		private GameObject fingerStart;
		public Canvas LetterQuery;
		public Text FeedbackText;
		public InputField LetterAnswer;
		private string answer;
		public List<string> experimentConfigList;
		private Coroutine streamRoutine;

		private float trial_start_time;
		private float image_presentation_time;
		private float finger_lift_time;
		private float finger_first_touch_time;
		private float finger_touch_time;

		// tracking current trial information
		private int _absolute_trial_number;
		private bool fingerOnStart = false;
		private bool eye_on_fixation = true;
		private bool good_trial = true;
		private bool stream_complete = false;
		private string[] charStream;
		private Vector3 first_touchpoint;
		private Vector3 touchpoint;

		public int Absolute_trial_number
		{
			get
			{
				return _absolute_trial_number;
			}

			set
			{
				Assert.IsTrue(value >= 0, "Absolute trial number negative!");
				// Assert.IsTrue(value < experimentConfig.Count, "Absolute trial number is larger than number of trials!");
				_absolute_trial_number = value;
			}
		}

		#region UnityEvents
		// Use this for initialization
		void Start()
		{
			LetterQuery.enabled = false;

			// Asset bundle stuff
			assetPath = Application.persistentDataPath + "/images";
			imageAssets = AssetBundle.LoadFromFile(assetPath);

			// Trial Tracking (bad/good)
			Absolute_trial_number = PlayerPrefs.GetInt("line", 0);
			experimentConfigFilename = Application.persistentDataPath + "/" + PlayerPrefs.GetString("configName", "config_spider.txt");
			Debug.Log("ReadConfigFile");
			Debug.Log("Trial Number: " + Absolute_trial_number.ToString());
			Debug.Log(PlayerPrefs.GetString("bad", ""));
			Debug.Log(PlayerPrefs.GetInt("badflag", 0));

			// Parse the config file
			experimentConfigList = new List<string>();
			experimentConfig = ReadConfigFile();
			PlayerPrefs.SetInt("block_fb", 0);
			PlayerPrefs.SetInt("block_percentage", 0);
			Debug.Log("LoadAsset");

			// set up fixation/home button and their size
			fingerStart = (GameObject)Instantiate(imageAssets.LoadAsset("home_button"));
			fingerStart.transform.position = new Vector3(0, -1f);
			Vector3 home_button_size = Vector3.Scale(fingerStart.GetComponent<Renderer>().bounds.size, new Vector3(Screen.height / Camera.main.orthographicSize / 2, Screen.height / Camera.main.orthographicSize / 2));
			Debug.Log(fingerStart.GetComponent<Renderer>().bounds.size);
			Debug.Log(home_button_size);
			Vector3 home_button_inverse = new Vector3(1 / home_button_size.x, 1 / home_button_size.y);
			Vector3 scalingFactor = new Vector3(0.6f * Screen.dpi / 2.54f, 0.6f * Screen.dpi / 2.54f);
			scalingFactor.Scale(home_button_inverse);
			fingerStart.transform.localScale = scalingFactor;
			Debug.Log(fingerStart.GetComponent<Renderer>().bounds.size);

			// network communications
			MATLABclient.mlClient.subject.AddObserver(this);

			// Initialize the state machine
			state = State.InitTrial;
			NextState();
		}

		// Update is called once per frame
		void Update()
		{
			// Keyboard tracking
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				SceneManager.LoadScene("start");
			}
			if (state >= State.InitTrial)
			{
				fingerOnStart = CheckMouseCollision(fingerStart.name);
			}
			if (state == State.Lifted)
			{
				if (Input.GetMouseButton(0))
				{
					if (first_touchpoint.Equals(Vector3.zero)) // first landing
					{
						first_touchpoint = Input.mousePosition;
						finger_first_touch_time = Time.time - trial_start_time;
					}
					else // save the latest touchpoint
					{
						touchpoint = Input.mousePosition;
						finger_touch_time = Time.time - trial_start_time;
					}
				}
				// record the touch here
			}
		}

		private void OnApplicationQuit()
		{
			PlayerPrefs.SetInt("lastBlockLine", PlayerPrefs.GetInt("line", 0));
			MATLABclient.mlClient.SendExit();
			// cleanup stuff goes here
		}

		private void OnDestroy()
		{
			imageAssets.Unload(true);
			PlayerPrefs.SetInt("endNum", PlayerPrefs.GetInt("line", 0));
			MATLABclient.mlClient.subject.RemoveObserver(this);
			PlayerPrefs.SetInt("lastBlockLine", PlayerPrefs.GetInt("line", 0));
		}

		private void FixedUpdate()
		{
		}

		private void OnApplicationPause(bool pause)
		{
			PlayerPrefs.SetInt("lastBlockLine", PlayerPrefs.GetInt("line", 0));
		}
		#endregion UnityEvents

		#region characterStream
		// Flashing Text assets
		public Text charStreamText;
		private const string letters = "KNVXYZ";
		private string targetLetter = " ";
		private int target_index;
		private float letter_time;

		IEnumerator StreamChars(string[] charStream, float displayDuration, float displayInterval)
		{
			foreach (string character in charStream)
			{
				// display the letter at that position
				this.charStreamText.text = character;
				this.charStreamText.enabled = true;
				float displayStart = Time.time;
				if (character.Equals(targetLetter))
				{
					letter_time = displayStart - trial_start_time;
				}
				yield return new WaitForSeconds(displayDuration);
				// turn off the letter
				this.charStreamText.enabled = false;
				float actualDuration = Time.time - displayStart;
				yield return new WaitForSeconds(displayInterval - actualDuration); // use actualDuration to maintain interval length as much as possible
			}
			stream_complete = true;
		}

		public string[] MakeCharStream(string target_letter, int target_index, int repeat_distance)
		{
			Assert.IsTrue(target_index >= 0);
			string[] generatedString = new string[target_index + 14];
			Queue<string> previous_chars = new Queue<string>(repeat_distance);
			for (int i = 0; i < generatedString.Length; i++)
			{
				string tmp;
				if (i == target_index)
				{
					tmp = target_letter;
				}
				else
				{
					tmp = Random.Range(0, 10).ToString(); // all the single digit numbers
					while (previous_chars.Contains(tmp)) // until there's no string that's the same as the first two
					{
						tmp = Random.Range(0, 10).ToString();
					}
				}
				if (previous_chars.Count >= repeat_distance)
				{
					previous_chars.Dequeue();
				}
				previous_chars.Enqueue(tmp); // last two characters
				generatedString[i] = tmp;
			}
			return generatedString;
		}

		/**
		 * Inserts a letter into the array of characters between 5 and the end
		 * of the array. Sets the letter in this instance.
		 * 
		 * Parameters: charStream -- Stream of number characters to be presented
		 * Output: modified charStream with Target letter inside
		 * Effects: Inserts Target Letter into charStream.
		 */
		public string PickLetter(out int target_index)
		{
			target_index = Random.Range(5, 11); // after 5-10 digits presented + 0 indexing
			return letters[Random.Range(0, letters.Length)].ToString();
		}
		#endregion characterStream

		#region setup stuff
		public List<TrialConfig> ReadConfigFile()
		{

			try
			{
				List<TrialConfig> trialConfigs = new List<TrialConfig>();
				StreamReader configStream = new StreamReader(experimentConfigFilename);

				while (!configStream.EndOfStream)
				{
					string configLine = configStream.ReadLine();
					experimentConfigList.Add(configLine);
					trialConfigs.Add(new TrialConfig(configLine));
				}
				configStream.Close();
				return trialConfigs;
			}
			catch (System.Exception e)
			{
				ExitBlock(9); // 9 = fileread error
				Debug.Log("Config file error: " + e);
				return null;
			}

		}
		#endregion

		#region behaviour
		public override void OnNotify()
		{
			Debug.Log("Reading response");
			string response = MATLABclient.mlClient.GetResponse();
			if (response.Contains("Fixation"))
			{
				eye_on_fixation = true;
			}
			else if (response.Contains("Restart"))
			{
				if (state >= State.InitTrial && state < State.AskForLetter)
				// once you ask for the letter don't check the eye anymore
				{
					eye_on_fixation = false;
					good_trial = false;
				}
			}
			Debug.Log(response);
		}

		public bool CheckMouseCollision(string objectName)
		{
			if (Input.GetMouseButton(0))
			{
				Vector3 mouse_pos = Input.mousePosition;
				RaycastHit2D collision = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mouse_pos), Vector2.zero);
				if (collision.collider != null && collision.collider.name == objectName)
				{
					return true;
				}
				else { return false; }
			}
			else { return false; }
		}
		#endregion behaviour


		#region State Machine
		public enum State
		{
			Start,
			InitTrial,
			PreImage,
			Image,
			Lifted,
			AskForLetter,
			Feedback
		} // state of the trial

		public State state;
		void NextState()
		{
			Debug.Log("NextState");
			string method_name = state.ToString();
			System.Reflection.MethodInfo info =
				GetType().GetMethod(method_name,
					System.Reflection.BindingFlags.NonPublic |
					System.Reflection.BindingFlags.Instance);
			StartCoroutine((IEnumerator)info.Invoke(this, null));
		}

		IEnumerator InitTrial()
		{

			// reset trial variables
			good_trial = true;
			if (MATLABclient.mlClient.SocketReady)
			{
				eye_on_fixation = false;
			}
			else eye_on_fixation = true;
			stream_complete = false;
			charStreamText.text = "+";

			// reset all the variables we're saving
			finger_touch_time = 0;
			finger_first_touch_time = 0;
			finger_lift_time = 0;
			touchpoint = Vector3.zero;
			first_touchpoint = Vector3.zero;



			// set trial character stream
			targetLetter = PickLetter(out target_index);
			charStream = MakeCharStream(targetLetter, target_index, 2);

			// figure out if we're starting the bad set or still running new trials
			if (PlayerPrefs.GetInt("badflag", 0) == 1)
			{
				if (PlayerPrefs.GetString("bad", "") != "")
				{
					badTrials = new List<string>(PlayerPrefs.GetString("bad", "").Split(';'));
					// trialEvents =
					int badtrial = Random.Range(0, badTrials.Count);
					current_trial_config = new TrialConfig(badTrials[badtrial]);
					badTrials.Remove(badTrials[badtrial]);
					string badTrialsArray = string.Join(";", badTrials.ToArray());
					PlayerPrefs.SetString("bad", badTrialsArray);
					trialEvents = current_trial_config.LoadStimuli(imageAssets);
				}
				else if (Absolute_trial_number >= experimentConfigList.Count) // we're done with all the bad flags trials! Exit the block.
				{
					ExitBlock(9); // finished experiment
				}
				else
				{
					ExitBlock(0); // finished block
				}
			}
			else // not in bad mode at the moment
			{
				current_trial_config = experimentConfig[Absolute_trial_number];
				trialEvents = current_trial_config.LoadStimuli(imageAssets);
			}

			// re-display home key, turn off images
			fingerStart.GetComponent<Renderer>().enabled = true;
			this.charStreamText.enabled = true;

			Debug.Log("Initialization complete");
			// wait for finger to be on the start key
			while (!fingerOnStart)
			{
				yield return 0;
			}


			// send the Eyelink Signal
			string trialString = "(" + current_trial_config.Config[(int)TrialConfig.ConfigIndex.block] + "," +
				current_trial_config.Config[(int)TrialConfig.ConfigIndex.trial] + ")";
			MATLABclient.mlClient.SendEyelinkBegin(trialString);

			while (!eye_on_fixation) // wait for the eye to be on the fixation
			{
				yield return 0;
			}
			trial_start_time = Time.time;
			Debug.Log("Fixation received");
			state = State.PreImage;
			NextState();
		}

		private IEnumerator PreImage()
		{
			Debug.Log("PreImage");
			// start the character stream
			streamRoutine = StartCoroutine(StreamChars(charStream, 0.03f, 0.1f));
			float startTime = Time.time;
			float imageOnset = target_index * 0.1f + 0.100f + (float)Random.Range(0, 2) * 0.2f;
			while (Time.time - startTime < imageOnset)
			{
				// if at any point before the image shows up, the
				// eye moves from the fixation or the finger lifts
				// it's a bad trial
				if (!eye_on_fixation || !fingerOnStart)
				{
					if (!eye_on_fixation)
					{
						Debug.Log("Eye isn't on fixation!");
					}
					if (!fingerOnStart)
					{
						Debug.Log("Finger not on start!");
					}
					MATLABclient.mlClient.SendRestart();
					good_trial = false;
					break;
				}
				else
				{
					// keep goin'
					yield return 0;
				}
			}
			if (!good_trial)
			{
				StopCoroutine(streamRoutine);
				state = State.Feedback;
			}
			else
			{
				MATLABclient.mlClient.SendOptotrackBegin();
				state = State.Image;
			}
			NextState();
		}

		private IEnumerator Image()
		{
			trialEvents[0].GetComponent<Renderer>().enabled = true;
			image_presentation_time = Time.time - trial_start_time;
			float startTime = Time.time;
			// if the stream finishes before the person lifts their finger, that's a bad trial
			while (!stream_complete)
			{
				if (!good_trial || !eye_on_fixation)
				{
					trialEvents[0].GetComponent<Renderer>().enabled = false;
					state = State.Feedback;
					StopCoroutine(streamRoutine);
					break;
				}
				else if (!fingerOnStart) // let's just say that if the finger moves away from the start,
										 // that they're moving towards the stimulus
				{
					finger_lift_time = Time.time - trial_start_time;
					state = State.Lifted;
					break;
				}
				else { yield return 0; }
			}
			if (state == State.Image) // they kept their eye and finger on initial position the whole time
			{
				MATLABclient.mlClient.SendTrialEnd();
				state = State.Feedback;
				good_trial = false;
			}
			NextState();
		}

		private IEnumerator Lifted()
		{
			Debug.Log(state.ToString());

			fingerStart.GetComponent<Renderer>().enabled = false;
			trialEvents[0].GetComponent<Renderer>().enabled = false;
			trialEvents[1].GetComponent<Renderer>().enabled = true;
			while (!stream_complete)
			{
				yield return 0;
			}

			Debug.Log("Turning off renderer for shape");
			trialEvents[1].GetComponent<Renderer>().enabled = false;
			MATLABclient.mlClient.SendTrialEnd();
			yield return new WaitForSeconds(0.5f);
			state = State.AskForLetter;
			NextState();
		}

		IEnumerator AskForLetter()
		{
			Debug.Log(state.ToString());
			// turn off the fixation and home key, if they aren't already
			if (int.Parse(current_trial_config.
				Config[(int)TrialConfig.ConfigIndex.ask_for_target]) > 0) // if we are asking for the target
			{
				// not implemented yet
				answer = "";
				LetterQuery.enabled = true; // this shows the prompt for the letter
				LetterAnswer.Select();
				LetterAnswer.ActivateInputField();
				do
				{
					if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) // waiting for subject to press enter
					{
						answer = LetterAnswer.text;
						if (answer.Length > 1)
						{
							answer = "";
							LetterAnswer.text = "";
						}
					}
					yield return 0;
				} while (answer == "");

				LetterAnswer.text = ""; // always clear the field
				LetterQuery.enabled = false;
				state = State.Feedback;
			}
			else
			{
				state = State.Feedback;
			}
			NextState();
		}

		IEnumerator Feedback()
		{
			Debug.Log("Feedback");
			charStreamText.text = "";
			fingerStart.GetComponent<Renderer>().enabled = false;
			for (var i = 0; i < trialEvents.Count; i++)
			{
				trialEvents[i].GetComponent<Renderer>().enabled = false;
			}
			// use the boolean variables to see if there's been anything bad happening
			if (!good_trial || first_touchpoint == Vector3.zero)
			{
				FeedbackText.text = "Bad trial!";
				AddBadTrial(string.Join(",", current_trial_config.Config.ToArray()));
				// say it was a good trial
			}
			else
			{
				FeedbackText.text = "Good trial!";
				// something something good job!!
			}
			AddData(); // add the data regardless, we can filter out the bad ones ourselves
			yield return new WaitForSeconds(1.5f);
			FeedbackText.text = "";
			// turn off the GUI elements
			state = State.InitTrial;
			Advance();
		}

		void Advance()
		{
			// check if it was a bad trial, and add it to the set of bad trials
			if (!good_trial)
			{
				Debug.Log("Adding bad trial to list of bad trials");
				AddBadTrial(string.Join(",", current_trial_config.Config.ToArray()));
			}
			Debug.Log(state.ToString());
			// unload the previous assets
			for (int i = 0; i < trialEvents.Count; i++)
			{
				Object.Destroy(trialEvents[i]);
			}
			List<string> previous_trial = current_trial_config.Config;

			// move on to the next trial
			if (PlayerPrefs.GetInt("badflag", 0) == 0)
			{
				Absolute_trial_number++;
				PlayerPrefs.SetInt("line", PlayerPrefs.GetInt("line") + 1);
				PlayerPrefs.SetInt("lastBlockLine", PlayerPrefs.GetInt("line", 0));
			}
			// if we're at the end of a block or the end of the experiment, run the bad trials again
			if (Absolute_trial_number >= experimentConfigList.Count) // end of experiment
			{
				// we don't have any more blocks;
				PlayerPrefs.SetInt("badflag", 1); // run all the bad blocks
				Debug.Log("Rerunning bad trials");
			}
			else
			{ // still more lines, check if it's end of block
				List<string> next_trial = experimentConfig[Absolute_trial_number].Config;
				if (int.Parse(next_trial[(int)TrialConfig.ConfigIndex.block]) > int.Parse(previous_trial[(int)TrialConfig.ConfigIndex.block]))
				{
					Debug.Log("Block Complete");
					Debug.Log("Rerunning bad trials");
					PlayerPrefs.SetInt("badflag", 1); // run all the bad blocks
				}
			}

			state = State.InitTrial;
			SceneManager.LoadScene("tryingStringStream");
		}
		#endregion

		public void AddData()
		{
			string data = PlayerPrefs.GetString("Data", "");
			if (!data.Equals(""))
			{
				data += ";"; // trial delimiter ";"
			}
			good_trial = good_trial && !first_touchpoint.Equals(Vector3.zero);
			string delimiter = "\t";
			string this_trial_data = "";
			this_trial_data += current_trial_config.Config[(int)TrialConfig.ConfigIndex.trial] + delimiter;
			this_trial_data += good_trial.ToString() + delimiter; // was it okay?
			this_trial_data += targetLetter + delimiter; // what's the target
			this_trial_data += target_index.ToString() + delimiter; // which character position was it?
			this_trial_data += letter_time.ToString() + delimiter;
			this_trial_data += answer + delimiter; // what did the subject answer?
			this_trial_data += image_presentation_time.ToString() + delimiter; // time for presentation of first event
			this_trial_data += finger_lift_time.ToString() + delimiter; // time for finger to lift from presentation of first event
			this_trial_data += finger_first_touch_time.ToString() + delimiter;
			this_trial_data += first_touchpoint.x.ToString() + delimiter;
			this_trial_data += first_touchpoint.y.ToString() + delimiter;
			this_trial_data += finger_touch_time.ToString() + delimiter; // when did the finger land on the screen again
			this_trial_data += touchpoint.x.ToString() + delimiter;
			this_trial_data += touchpoint.y.ToString() + delimiter;
			foreach (GameObject trialEvent in trialEvents)
			{
				Vector3 image_position = Camera.main.WorldToScreenPoint(trialEvent.transform.localPosition);
				this_trial_data += image_position.x.ToString() + delimiter;
				this_trial_data += image_position.y.ToString() + delimiter;
			}

			data += this_trial_data; // in debug mode this makes it easier to read;

			PlayerPrefs.SetString("Data", data);
			// save the following:
			// letter number (when the image was presented)
			// the letter itself
			// the answer
			// when the finger was lifted
			// when the finger pressed down (if it did)
			// touchpoint X
			// touchpoint Y
		}

		public void AddBadTrial(string trialConfig)
		{
			string badTrials = PlayerPrefs.GetString("bad", "");
			if (badTrials != "")
			{
				badTrials += ";";
			}
			if (!badTrials.Contains(trialConfig))
			{
				badTrials += trialConfig;
				PlayerPrefs.SetString("bad", badTrials);
			}
		}

		public void ExitBlock(int flag)
		{
			PlayerPrefs.SetInt("badflag", 0);
			PlayerPrefs.SetInt("exit_flag", flag);
			PlayerPrefs.SetInt("lastBlockLine", PlayerPrefs.GetInt("line", 0));
			SceneManager.LoadScene("End");
		}
	}
}