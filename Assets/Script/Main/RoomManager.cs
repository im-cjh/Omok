using System;
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
        try
        {
            _session = FindObjectOfType<Session>();
            _session.enterRoomRecvEvent += OnPlayerEntered;

            users = new RoomUser[2];
            GameObject tmp = gameObject.transform.GetChild(1).gameObject;
            users[0] = tmp.GetComponent<RoomUser>();
            
            tmp = gameObject.transform.GetChild(2).gameObject;
            users[1] = tmp.GetComponent<RoomUser>();

            roomName.text = LobbyManager.Instance.GetSelectedRoom().roomName;
        }
        catch (Exception e)
        {
            Debug.Log("user error");
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void QuitRoom(User user)
    {
        _session
    }

    public void OnPlayerQuit(Protocol.P_Player pPlayer)
    {
        throw new NotImplementedException();
    }

    public void OnPlayerEntered(List<Protocol.P_Player> pPlayers)
    {
        //Debug.Log(pPlayers.Count);

        for(int i = 0; i < pPlayers.Count; i++) 
        {
            if (pPlayers[i].UserName == null)
            {
                Debug.Log("s");
            }
            else
            {
                Debug.Log("e");
                users[i].SetInfo(pPlayers[i].UserName);
            }
        }
    }
}
