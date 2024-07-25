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
        userInfos = new RoomUser[2];
        
        //LobbySession.Instance.enterRoomRecvEvent += OnPlayerEntered;
        //LobbySession.Instance.quitRoomRecvEvent += OnPlayerQuit;

        BattleSession.Instance.enterFastRoomEvent += OnPlayerEntered;
        BattleSession.Instance.gameSetEvent += OnGameSet;
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

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                //·±Å¸ÀÓ¿¡ ÇÒ´ç
                roomName = Utilities.FindAndAssign<Text>("Canvas/UserInfo/RoomName");
                userInfos[0] = Utilities.FindAndAssign<RoomUser>("Canvas/UserInfo/User1");
                userInfos[1] = Utilities.FindAndAssign<RoomUser>("Canvas/UserInfo/User2");
                
                
                for (int i = 0; i < 2; i += 1)
                {
                    userInfos[i].SetInfo(pPlayers.Players[i].UserName);
                    if (pPlayers.Players[i].StoneType == 1)
                    {
                        userInfos[i].SetStoneColor(Color.black);
                        if(User.Instance.userName == pPlayers.Players[i].UserName)
                        {
                            ClickHandler.Instance.SetStoneColor(Color.black);
                            ClickHandler.Instance._myTurn = true;
                        }
                    }
                    else if (pPlayers.Players[i].StoneType == 2)
                    {
                        userInfos[i].SetStoneColor(Color.white);
                        if (User.Instance.userName == pPlayers.Players[i].UserName)
                            ClickHandler.Instance.SetStoneColor(Color.white);
                    }
                }

                
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });  
    }
    public void OnGameSet(eStone pStoneType)
    {
        Debug.Log("OnGameSet");

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                ChatStruct chatStruct = new ChatStruct();
                chatStruct.name = "[System]";
                chatStruct.content = pStoneType == eStone.BLACK ? "Èæµ¹ ½Â" : "¹éµ¹ ½Â";
                chatStruct.color = Color.red;
                ChatManager.Instance.UpdateChat(chatStruct);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
    }
}
