using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StimulusEvent : MonoBehaviour {

    private GameObject stimulusObject;
    private float scale;
    private float rotation;
    private Vector3 position;
    private float presentation_time;
    private float onset_time;
    private float total_time;

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

    public void SetupStimulus(string stim_name, float sca, float rot, float pos_x, float pos_y, float p_t, float o_t, float t_t)
    {
        scale = sca;
        rotation = rot;
        position = new Vector3(pos_x, pos_y);
        presentation_time = p_t;
        onset_time = o_t;
        total_time = t_t;
        stimulusObject = (GameObject) AssetBundle.LoadFromFile(Application.persistentDataPath + "/images").LoadAsset(stim_name);
        Resize();
    }

    public IEnumerator Present()
    {
        float startTime = Time.time;
        stimulusObject.GetComponent<Renderer>().enabled = true;
        yield return new WaitForSeconds(presentation_time);
        stimulusObject.GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(total_time - (Time.time - startTime));
    }

    public void Resize()
    {
        Renderer stimulusRenderer = stimulusObject.GetComponent<Renderer>();
        stimulusRenderer.enabled = false;
        float screenHeight = 2f * Camera.main.orthographicSize;
        float screenWidth = screenHeight * Camera.main.aspect;
        float newImageDiag = screenHeight * scale / 100;
        //float maxImageWidth = screenWidth * scale / 100;

        // Resize the stimulus to the screen
        Vector2 img_dim = new Vector2(stimulusRenderer.bounds.size.x, stimulusRenderer.bounds.size.y);
        float diag = img_dim.magnitude;
        float scaleFactor = diag / newImageDiag;
        stimulusObject.transform.localScale.Scale(new Vector3(scaleFactor, scaleFactor, 1));

        float pixelOrthoRatio = Screen.height / 2f / Camera.main.orthographicSize;
        float border = 3 * Screen.dpi / pixelOrthoRatio / 8; // 3/8ths of an inch on the edges
        float margin_x = screenWidth - newImageDiag - border;
        float margin_y = screenHeight - newImageDiag - border;
        stimulusObject.transform.position = Vector3.Scale(position, new Vector3(margin_x/100f,margin_y/100f));

        stimulusObject.transform.eulerAngles = new Vector3(0, 0, rotation);
    }


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
