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
    public static User _instance;

    public string userName;
    public int id;

    public static User Instance
    {
        get
        {
            if (_instance == null)
            {
                // Scene에서 RoomManager를 찾아 인스턴스화
                _instance = FindObjectOfType<User>();
                if (_instance == null)
                {
                    // RoomManager가 없는 경우 새로 생성
                    GameObject obj = new GameObject("user");
                    _instance = obj.AddComponent<User>();
                }
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }
}
