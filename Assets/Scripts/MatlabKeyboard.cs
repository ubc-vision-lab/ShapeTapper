using HD;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatlabKeyboard : Observer {

    public Text serverResponse;
    private Coroutine networkCoroutine;
    public TcpConnectedClient unityClient;
    public GameObject solo12;
    public GameObject solo11;
    public GameObject solo5;
    private int state = 0;
    float timetoimage = 0.7f;
    float onset_timer = 0f;
    float optotrack_time = 2f;
    float optotrack_timer = 0;
    public Text mousePositionText;

	// Use this for initialization
	void Start () {
        string assetPath = Application.persistentDataPath + "/images";
        AssetBundle assets = AssetBundle.LoadFromFile(assetPath);
        serverResponse.text = "";
        solo12 = (GameObject) Instantiate(assets.LoadAsset("solo12"));
        solo12.transform.localScale.Scale(new Vector3(0.25f, 0.25f, 1));
        solo12.transform.localPosition = new Vector3(0, 0, 0);
        solo12.GetComponent<Renderer>().enabled = true;

        solo11 = (GameObject)Instantiate(assets.LoadAsset("solo11"));
        solo11.transform.localScale.Scale(new Vector3(0.25f, 0.25f, 1));
        solo11.transform.localPosition = new Vector3(4, 3, 0);
        solo11.GetComponent<Renderer>().enabled = false;

        solo5 = (GameObject)Instantiate(assets.LoadAsset("solo5"));
        solo5.transform.localScale.Scale(new Vector3(0.25f, 0.25f, 1));
        solo5.transform.localPosition = new Vector3(4, -3, 0);
        solo5.GetComponent<Renderer>().enabled = false;
        MATLABclient.mlClient.subject.AddObserver(this);

        state = 0;
	}

    // Update is called once per frame
    void Update ()
    {

        switch (state)
        {
            case 0: // wait for homekey
                if (Input.GetMouseButton(0))
                {
                    Vector3 mouse_pos = Input.mousePosition;
                    mousePositionText.text = mouse_pos.ToString();
                    RaycastHit2D collision = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mouse_pos), Vector2.zero);
                    if (collision.collider != null && collision.collider.name == solo12.name)
                    {
                        MATLABclient.mlClient.SendEyelinkBegin();
                        state = 1;
                        onset_timer = Time.time;
                    }
                }
                break;
            case 1: // wait for timer to elapse
                if(!Input.GetMouseButton(0))
                {
                    MATLABclient.mlClient.SendTrialEnd();
                    state = 0;
                }
                else
                {
                    Vector3 mouse_pos = Input.mousePosition;
                    RaycastHit2D collision = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(mouse_pos), Vector2.zero);
                    if(collision.collider == null || collision.collider.name != solo12.name)
                    {
                        MATLABclient.mlClient.SendTrialEnd();
                        state = 0;
                    }
                    else if(Time.time-onset_timer > timetoimage)
                    {
                        MATLABclient.mlClient.SendOptotrackBegin();
                        solo11.GetComponent<Renderer>().enabled = true;
                        state = 2;
                        optotrack_timer = Time.time;
                    }
                }
                break;
            case 2:
                if(!Input.GetMouseButton(0))
                {
                    solo11.GetComponent<Renderer>().enabled = false;
                    state = 3;
                    solo5.GetComponent<Renderer>().enabled = true;
                }
                break;
            case 3:
                if(Input.GetMouseButton(0))
                {
                    Vector3 mouse_position = Input.mousePosition;
                    mousePositionText.text = mouse_position.ToString();
                }
                if (Time.time - optotrack_timer > optotrack_time)
                {
                    solo5.GetComponent<Renderer>().enabled = false;
                    MATLABclient.mlClient.SendTrialEnd();
                    state = 0;
                }
                break;
            default:
                break;
        }
            
        
        CheckKeys();
        //if (Input.GetKeyDown(KeyCode.T))
        //{
        //    if (networkCoroutine == null)
        //    {
        //        networkCoroutine = StartCoroutine(generateRecords());
        //    }
        //}
    }

    private void CheckKeys()
    {
        if(Input.GetKeyDown(KeyCode.E))
        {
            MATLABclient.mlClient.SendEyelinkBegin();
        }
        if(Input.GetKeyDown(KeyCode.R))
        {
            MATLABclient.mlClient.SendRestart();
        }
        if (Input.GetKeyDown(KeyCode.Z))
        {
            MATLABclient.mlClient.SendTrialEnd();
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            MATLABclient.mlClient.SendOptotrackBegin();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MATLABclient.mlClient.SendExit();
        }
    }

    IEnumerator generateRecords()
    {
        for(int i = 1; i <= 6; i++)
        {
            // serverResponse.text += i.ToString() + ": ";
            float startTime = Time.time;
            MATLABclient.mlClient.SendOptotrackBegin();
            yield return new WaitForSeconds(2*i);
            MATLABclient.mlClient.SendTrialEnd();
            float endTime = Time.time - startTime;
            // serverResponse.text += endTime.ToString() + "\n";
            yield return new WaitForSeconds(1);

        }
        MATLABclient.mlClient.SendExit();
    }

    public override void OnNotify()
    {
        serverResponse.text = MATLABclient.mlClient.GetResponse();

        //serverResponse.text = TCPChat.messageQueue.Peek();
        Debug.Log(serverResponse.text);
        //unityClient.Send("What happened?");
        //TCPChat.messageQueue.Dequeue();
    }
}