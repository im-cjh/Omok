using Protocol;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class RoomManager : MonoBehaviour
{
    [SerializeField]
    Text roomName;

    [SerializeField]
    RoomUser[] userInfos;

    private List<Protocol.P_LobbyPlayer> _players;
    // Start is called before the first frame update

   

    private void Awake()
    {
        LobbySession.Instance.enterRoomRecvEvent += OnPlayerEntered;
        LobbySession.Instance.quitRoomRecvEvent += OnPlayerQuit;

        BattleSession.Instance.enterRoomRecvEvent += OnPlayerEntered;
        BattleSession.Instance.quitRoomRecvEvent += OnPlayerQuit;
    }
    void Start()
    {
        _players = new List<Protocol.P_LobbyPlayer>(2);  
        try
        {
        

            userInfos = new RoomUser[2];
            GameObject tmp = gameObject.transform.GetChild(1).gameObject;
            userInfos[0] = tmp.GetComponent<RoomUser>();
            
            tmp = gameObject.transform.GetChild(2).gameObject;
            userInfos[1] = tmp.GetComponent<RoomUser>();

            roomName.text = LobbyManager.Instance.GetSelectedRoom().roomName;
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
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
        LobbySession.Instance.Send(sendBuffer);

        Protocol.C2SChatRoom pkt2 = new Protocol.C2SChatRoom();
        pkt2.RoomID = LobbyManager.Instance.GetSelectedRoom().roomId;
        pkt2.SenderName = "Someone";
        pkt2.Content = "Quit Room";

        sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.CHAT_MESSAGE);
        LobbySession.Instance.Send(sendBuffer);

        SceneChanger.ChangeLobbyScene();
    }

    public void OnPlayerQuit(Protocol.P_LobbyPlayer pPlayer)
    {
        Debug.Log("OnPlayerQuit");
        _players.Remove(pPlayer);
        UpdateUserInfo();
        //throw new NotImplementedException();
    }

    public void OnPlayerEntered(List<Protocol.P_LobbyPlayer> pPlayers)
    {
        //Debug.Log(pPlayers.Count);
        _players = pPlayers;
        //UpdateUserInfo();
        SceneChanger.ChangeGameScene();
    }

    private void UpdateUserInfo()
    {
        for (int i = 0; i < _players.Count; i+=1)
        {
            userInfos[i].SetInfo(_players[i].UserName);
        }
    }
}
