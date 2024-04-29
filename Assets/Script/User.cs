using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class User : MonoBehaviour
{
    public string userName;
    public int id;

    private Session _session;

    // Start is called before the first frame update
    void Start()
    {
        _session = FindObjectOfType<Session>();
        DontDestroyOnLoad(this);
    }
}
