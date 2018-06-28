using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EnnsLab
{
	/**
	 * Stimulus will display an image for the specified period of time. After the
	 * specified period passes, the image will be hidden from view.
	 * Each stimulus can have a mask, which will display in the same position
	 * as the image, but is not rotated nor scaled. Its display timing is
	 * independent of the image's timing.
	 */
	public class Stimulus : MonoBehaviour, IStimulus
	{
		[SerializeField] int eventNumber = 0;
		// maybe make the stimulusObject an arra instead.
		private List<GameObject> stimulusObjects;
		private GameObject mask; // TODO: Add the parsing for this
		private float scale;
		private float rotation;
		private Vector3 position;
		private float presentation_time;
		private float onset_time;
		private float total_time;

		private ExperimentConfig experimentConfig;

		#region Properties
		public float Rotation
		{
			get
			{
				return rotation;
			}

			set
			{
				rotation = value % 360f;
			}
		}

		public float Scale
		{
			get
			{

				return scale;
			}

			set
			{
				scale = value;
			}
		}

		public float Presentation_time
		{
			get
			{
				return presentation_time;
			}

			set
			{
				presentation_time = value;
			}
		}

		public float Onset_time
		{
			get
			{
				return onset_time;
			}

			set
			{
				onset_time = value;
			}
		}

		public float Total_time
		{
			get
			{
				return total_time;
			}

			set
			{
				total_time = value;
			}
		}

		public Vector3 Position
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

		public GameObject Mask
		{
			get
			{
				return mask;
			}

			set
			{
				mask = value;
			}
		}

		public List<GameObject> StimulusObjects
		{
			get
			{
				return stimulusObjects;
			}

			set
			{
				stimulusObjects = value;
			}
		}

		public int EventNumber
		{
			get
			{
				return eventNumber;
			}

			set
			{
				eventNumber = value;
			}
		}
		#endregion

		/**
		 * Fetch information for the stimulus from the configuration singleton. If
		 * there is no data for this, then no image is loaded.
		 */
		public void SetupStimulus()
		{
			TrialConfig currentConfig = ExperimentConfig.instance.GetCurrentConfig();
			List<StimulusInfo> stimInfo = currentConfig.ParseAllStimuli();
			try
			{
				SetupStimulus(stimInfo[EventNumber]);
			}
			catch(Exception e)
			{
				Debug.Log(e.Message);
				Debug.Log(EventNumber);
			}
		}

		/** Setup stimulus given the information for the stimulus and mask
		 * 
		 */
		public void SetupStimulus(List<string> stim_names, float sca,
			float rot, float pos_x, float pos_y, float p_t, float o_t, float t_t)
		{
			Scale = sca;
			Rotation = rot;
			Position = new Vector3(pos_x, pos_y);
			Presentation_time = p_t;
			Onset_time = o_t;
			Total_time = t_t;
			LoadStimulusGameObjects(stim_names);
			ResizeAll();
		}

		/**
		 * Given stimulus information, constructs a stimulus that matches that info.
		 * 
		 */
		public void SetupStimulus(StimulusInfo stim_info)
		{
			SetupStimulus(stim_info.StimulusNames, stim_info.Scaling, stim_info.Rotation,
				stim_info.Position.x, stim_info.Position.y, stim_info.Display_interval,
				0f, stim_info.Display_interval + stim_info.Interstimulus_time);
		}

		protected void LoadStimulusGameObjects(List<string> stim_names)
		{
			Debug.Log("Loading Stimulus GameObjects...");
			foreach (string stim_name in stim_names)
			{
				Debug.Log("Adding " + stim_name);
				// formatting?
				StimulusObjects.Add(
					Instantiate<GameObject>(experimentConfig.assetBundle.LoadAsset<GameObject>(stim_name))
				);
				Debug.Log("Added " + stim_name);
			}
			Debug.Log("Stimulus GameObjects Loaded");
		}

		public IEnumerator Stimulate()
		{
			yield return new WaitForSecondsRealtime(Onset_time);
			float startTime = Time.time;
			foreach (GameObject stimulusObject in StimulusObjects)
			{
				stimulusObject.GetComponent<Renderer>().enabled = true;
				yield return new WaitForSecondsRealtime(Presentation_time);
				stimulusObject.GetComponent<Renderer>().enabled = false;
			}
			yield return new WaitForSecondsRealtime(Total_time - (Time.time - startTime));
		}


		/* Resize - 
		 *   resizes the image to the size specified in on creation of Stimulus
		 *   instance.
		 */
		public void ResizeAll()
		{
			// applies to all images
			float screenHeight = 2f * Camera.main.orthographicSize;
			float screenWidth = screenHeight * Camera.main.aspect;
			float newImageDiag = screenHeight * scale / 100;
			//float maxImageWidth = screenWidth * scale / 100;

			float pixelOrthoRatio = Screen.height / 2f / Camera.main.orthographicSize;
			float border = 3 * Screen.dpi / pixelOrthoRatio / 8; // 3/8ths of an inch on the edges
			float margin_x = screenWidth - newImageDiag - border;
			float margin_y = screenHeight - newImageDiag - border;

			foreach (GameObject stimulusObject in StimulusObjects)
			{
				Renderer stimulusRenderer = stimulusObject.GetComponent<Renderer>();
				stimulusRenderer.enabled = false;

				// Resize the stimulus to the screen
				Vector2 img_dim = new Vector2(stimulusRenderer.bounds.size.x, stimulusRenderer.bounds.size.y);
				float diag = img_dim.magnitude;
				float scaleFactor = diag / newImageDiag;
				stimulusObject.transform.localScale.Scale(new Vector3(scaleFactor, scaleFactor, 1));

				stimulusObject.transform.position = Vector3.Scale(Position, new Vector3(margin_x / 100f, margin_y / 100f));
				stimulusObject.transform.position = stimulusObject.transform.position - new Vector3(margin_x / 2, margin_y / 2, 0);

				stimulusObject.transform.eulerAngles = new Vector3(0, 0, Rotation);
			}
		}

		#region UnityLoop
		void Awake()
		{
			StimulusObjects = new List<GameObject>();
			experimentConfig = FindObjectOfType<ExperimentConfig>();
			if(experimentConfig == null)
			{
				Debug.Log("No experiment configuration object found.");
			}
		}

		void OnEnable()
		{
			// needs to search for ExperimentConfig object
			SetupStimulus();
			AbstractPresenter.ToBePresented.Add(this);
		}
		#endregion UnityLoop
	}
}