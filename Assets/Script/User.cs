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
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class User : MonoBehaviour
{
    public string userName;

    private Session _session;

    // Start is called before the first frame update
    void Start()
    {
        _session = FindObjectOfType<Session>();
        DontDestroyOnLoad(this);
    }
}
