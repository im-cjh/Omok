using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    Text roomName;

    [SerializeField]
    RoomUser[] users;


    private Session _session;
    // Start is called before the first frame update
    void Start()
    {
        _session = FindObjectOfType<Session>();
        _session.enterRoomRecvEvent += OnPlayerEntered;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPlayerEntered(List<Protocol.P_Player> pPlayers)
    {
        for(int i = 0; i < pPlayers.Count; i++) 
        {
            users[i].SetInfo(pPlayers[i].UserName);
        }
        Debug.Log("s");
    }
}
