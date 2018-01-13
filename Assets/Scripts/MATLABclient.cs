using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.IO;
using System;
using UnityEngine.UI;
using System.Net;

public class MATLABclient : MonoBehaviour {

    public static MATLABclient mlClient;
    private bool socketReady;
    
    private TcpClient socket;

    private NetworkStream stream;
    private StreamWriter writer;
    private StreamReader reader;
    public Text connectionText;

    public Subject subject = new Subject();

    public bool SocketReady
    {
        get
        {
            return socketReady;
        }

        set
        {
            socketReady = value;
        }
    }

    #region setup
    
    public void ConnectToMatlab()
    {
        if (SocketReady) { return; }

        string host = "127.0.0.1";
        int port = 56789;
        //int p;
        //int.TryParse(GameObject.Find("PortInput").GetComponent<InputField>().text, out p);
        //if (p != 0)
        //    port = p;

        try
        {
            socket = new TcpClient(host, port);
            stream = socket.GetStream();
            writer = new StreamWriter(stream);
            reader = new StreamReader(stream);
            SocketReady = true;
            SetStatusConnected();
            Debug.Log("MATLABclient started.");
        }
        catch(Exception e)
        {
            Debug.Log("Socket error: " + e.Message);
        }
    }

    internal void SendEyelinkBegin()
    {
        SendData("Eyelink");
    }

    internal void SendRestart()
    {
        SendData("Restart");
    }
    #endregion setup

    #region UnityEvents
    void Awake()
    {
        if (mlClient != null)
        {
            Destroy(this);
        }
        else
        {
            this.ConnectToMatlab();
            this.subject = new Subject();
            mlClient = this;
            DontDestroyOnLoad(this);
        }
    }
    // Use this for initialization
    void Start () {
    }
	
    // Update is called once per frame
    void Update () {
        if (SocketReady && stream != null && stream.DataAvailable)
        {
            subject.Notify();
        }
    }
    #endregion UnityEvents

    #region Data Handling
    private void OnIncomingData(string data)
    {
        Debug.Log("Response: " + data);
    }

    private void SendData(string data)
    {
        if (SocketReady)
        {
            writer.WriteLine(data);
            writer.Flush();
            Debug.Log("Message " + data + " sent.");
        }
    }

    internal void SendOptotrackBegin()
    {
        SendData("Optotrack");
    }

    internal void SendTrialEnd()
    {
        SendData("End");
    }

    internal void SendExit()
    {
        SendData("Exit");
    }

    internal bool HasResponse()
    {
        return reader.Peek() >= 0;
    }

    internal string GetResponse()
    {
        if(SocketReady)
        {
            if(reader.Peek() >= 0)
            {
                return reader.ReadLine();
            }
        }
        return "";
    }
    #endregion

    #region UI
    private void SetConnectionStatus(string status, Color color)
    {
        connectionText.text = "Server :" + status;
        connectionText.color = color;
    }

    private void SetStatusConnected()
    {
        SetConnectionStatus("Connected", Color.green);
    }

    private void SetStatusNotConnected()
    {
        SetConnectionStatus("Not connected", Color.red);
    }
    #endregion

}
