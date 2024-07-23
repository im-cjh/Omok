using PimDeWitte.UnityMainThreadDispatcher;
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

    //private List<Protocol.P_LobbyPlayer> _players;
    //private List<Protocol.P_LobbyPlayer> _players;

    // Start is called before the first frame update

    

    private void Start()
    {
        //LobbySession.Instance.enterRoomRecvEvent += OnPlayerEntered;
        //LobbySession.Instance.quitRoomRecvEvent += OnPlayerQuit;

        BattleSession.Instance.enterFastRoomEvent += OnPlayerEntered;
        DontDestroyOnLoad(this);
        //BattleSession.Instance.enterRoomRecvEvent += OnPlayerEntered;
        //BattleSession.Instance.quitRoomRecvEvent += OnPlayerQuit;
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
        //_players.Remove(pPlayer);
        //UpdateUserInfo();
        //throw new NotImplementedException();
    }
    public void OnPlayerEntered(Protocol.S2CGameStart pPlayers)
    {
        Debug.Log("OnPlayerEntered") ;

        userInfos = new RoomUser[2];
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {

                roomName = Utilities.FindAndAssign<Text>("Canvas/UserInfo/RoomName");
                userInfos[0] = Utilities.FindAndAssign<RoomUser>("Canvas/UserInfo/User1");
                userInfos[1] = Utilities.FindAndAssign<RoomUser>("Canvas/UserInfo/User2");

                for (int i = 0; i < 2; i += 1)
                {
                    userInfos[i].SetInfo(pPlayers.Players[i].UserName);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
        Debug.Log("OnMainSceneLoaded End");
    }
    //private void UpdateUserInfo()
    //{
    //    for (int i = 0; i < _players.Count; i+=1)
    //    {
    //        userInfos[i].SetInfo(_players[i].UserName);
    //    }
    //}
}
