using Protocol;
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
    RoomUser[] userInfos;

    private List<Protocol.P_Player> _players;
    // Start is called before the first frame update
    void Start()
    {
        _players = new List<Protocol.P_Player>(2);  
        try
        {
            Session.Instance.enterRoomRecvEvent += OnPlayerEntered;
            Session.Instance.quitRoomRecvEvent += OnPlayerQuit;

            userInfos = new RoomUser[2];
            GameObject tmp = gameObject.transform.GetChild(1).gameObject;
            userInfos[0] = tmp.GetComponent<RoomUser>();
            
            tmp = gameObject.transform.GetChild(2).gameObject;
            userInfos[1] = tmp.GetComponent<RoomUser>();

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

    public void QuitRoom()
    {
        Protocol.C2SQuitRoom pkt = new Protocol.C2SQuitRoom();
        pkt.RoomID = LobbyManager.Instance.GetSelectedRoom().roomId;
        pkt.UserID = User.Instance.id;

        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.QUIT_ROOM_MESSAGE);
        Session.Instance.Send(sendBuffer);

        SceneChanger.ChangeLobbyScene();
    }

    public void OnPlayerQuit(Protocol.P_Player pPlayer)
    {
        Debug.Log("OnPlayerQuit");
        _players.Remove(pPlayer);
        UpdateUserInfo();
        //throw new NotImplementedException();
    }

    public void OnPlayerEntered(List<Protocol.P_Player> pPlayers)
    {
        //Debug.Log(pPlayers.Count);
        _players = pPlayers;
        UpdateUserInfo();
    }

    private void UpdateUserInfo()
    {
        for (int i = 0; i < _players.Count; i+=1)
        {
            userInfos[i].SetInfo(_players[i].UserName);
        }
    }
}
