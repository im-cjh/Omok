using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEditor.PackageManager.Requests;

public enum eReqType
{
    Rooms
}

public class User : MonoBehaviour
{
    public string userName;

    private Session _session;

    // Start is called before the first frame update
    void Start()
    {
        _session = new Session();
        DontDestroyOnLoad(this);
    }

    public async Task ConnectToServer()
    {
        try
        {
            _session = new Session();
            await _session.Connect();
        }
        catch (Exception e) 
        {
            Debug.LogException(e);
        }
    }

    public async Task Request(eReqType pReqType)
    {
        switch (pReqType) 
        {
            case eReqType.Rooms:
                _session.RequestRooms();
                break;
        }
    }
}
