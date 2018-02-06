using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

public class SocketMATLAB : MonoBehaviour {

    #region remote server settings
    public IPAddress serverIP = IPAddress.Loopback; // assume it's on the same machine
    public int port = 56789; // matlab port
    #endregion remote server settings

    #region UnityEvents
    // Use this for initialization
    void Start () {
        TcpClient client = new TcpClient("What",port);
    }

    // Update is called once per frame
    void Update () {

    }
    #endregion

    #region API

    #endregion
}
