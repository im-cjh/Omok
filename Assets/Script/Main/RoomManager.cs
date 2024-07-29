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

    private static RoomManager _instance;
    //private List<Protocol.P_LobbyPlayer> _players;
    //private List<Protocol.P_LobbyPlayer> _players;

    // Start is called before the first frame update

    public static RoomManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<RoomManager>();
            
            return _instance;
        }
    }

    private void Start()
    {
        userInfos = new RoomUser[2];
        
        //LobbySession.Instance.enterRoomRecvEvent += OnPlayerEntered;
        //LobbySession.Instance.quitRoomRecvEvent += OnPlayerQuit;

        BattleSession.Instance.enterFastRoomEvent += OnPlayerEntered;
        BattleSession.Instance.gameSetEvent += OnGameSet;
        DontDestroyOnLoad(this);
        //BattleSession.Instance.enterRoomRecvEvent += OnPlayerEntered;
        BattleSession.Instance.quitRoomRecvEvent += OnPlayerQuit;
    }

    public void QuitCustomRoom()
    {
        Protocol.C2SQuitRoom pkt = new Protocol.C2SQuitRoom();
        pkt.RoomID = User.Instance.currentRoomID;
        pkt.UserID = User.Instance.id;

        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.QUIT_ROOM_MESSAGE);
        LobbySession.Instance.Send(sendBuffer);

        Protocol.C2SChatRoom pkt2 = new Protocol.C2SChatRoom();
        pkt2.RoomID = User.Instance.currentRoomID;
        pkt2.SenderName = User.Instance.userName;
        pkt2.Content = "가 방을 떠났습니다.";

        sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.CHAT_MESSAGE);
        LobbySession.Instance.Send(sendBuffer);

        SceneChanger.ChangeLobbyScene();
    }

    public void QuitFastRoom()
    {
        Protocol.C2SQuitRoom pkt = new Protocol.C2SQuitRoom();
        pkt.RoomID = User.Instance.currentRoomID;
        pkt.UserID = User.Instance.id;

        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.QUIT_ROOM_MESSAGE);
        BattleSession.Instance.Send(sendBuffer);

        Protocol.C2SChatRoom pkt2 = new Protocol.C2SChatRoom();
        pkt2.RoomID = User.Instance.currentRoomID;
        pkt2.SenderName = User.Instance.userName;
        pkt2.Content = "is Quit Room";

        sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.CHAT_MESSAGE);
        BattleSession.Instance.Send(sendBuffer);

        SceneChanger.ChangeLobbyScene();
        BattleSession.Instance.Disconnect();
    }

    public void OnPlayerQuit(Protocol.P_LobbyPlayer pPlayer)
    {
        Debug.Log("OnPlayerQuit");
        //userInfos.Remove(pPlayer);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                //런타임에 할당
                ChatManager.Instance.UpdateChat(new ChatStruct {  name = "[System]", content = pPlayer.UserName + " is Quit", color = Color.red});

            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
        
    }

    public void OnPlayerEnteredCustomRoom(Protocol.P_Player pPlayer)
    {
        Debug.Log("OnPlayerEnteredCustomRoom");

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                //런타임에 할당
                roomName = Utilities.FindAndAssign<Text>("Canvas/UserInfo/RoomName");
                userInfos[0] = Utilities.FindAndAssign<RoomUser>("Canvas/UserInfo/User1");
                userInfos[1] = Utilities.FindAndAssign<RoomUser>("Canvas/UserInfo/User2");


                for (int i = 0; i < 2; i += 1)
                {
                    userInfos[i].SetInfo(pPlayer.UserName);
                    userInfos[i].SetWinRate(pPlayer.Win, pPlayer.Lose);
                }
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
    }

    //플레이어 정보 기입
    public void OnPlayerEntered(Protocol.S2CGameStart pPlayers)
    {
        Debug.Log("OnPlayerEntered") ;

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            try
            {
                //런타임에 할당
                roomName = Utilities.FindAndAssign<Text>("Canvas/UserInfo/RoomName");
                userInfos[0] = Utilities.FindAndAssign<RoomUser>("Canvas/UserInfo/User1");
                userInfos[1] = Utilities.FindAndAssign<RoomUser>("Canvas/UserInfo/User2");
                
                
                for (int i = 0; i < 2; i += 1)
                {
                    userInfos[i].SetInfo(pPlayers.Players[i].UserName);
                    userInfos[i].SetWinRate(pPlayers.Players[i].Win, pPlayers.Players[i].Lose);

                    if (pPlayers.Players[i].StoneType == 1)
                    {
                        userInfos[i].SetStoneColor(Color.black);
                        if(User.Instance.userName == pPlayers.Players[i].UserName)
                        {
                            User.Instance.stoneColor = eStone.BLACK;
                            ClickHandler.Instance.SetStoneColor(Color.black);
                            ClickHandler.Instance._myTurn = true;
                        }
                    }
                    else if (pPlayers.Players[i].StoneType == 2)
                    {
                        userInfos[i].SetStoneColor(Color.white);
                        if (User.Instance.userName == pPlayers.Players[i].UserName)
                        {
                            User.Instance.stoneColor = eStone.WHITE;
                            ClickHandler.Instance.SetStoneColor(Color.white);
                        }
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
                chatStruct.content = pStoneType == eStone.BLACK ? "흑돌 승" : "백돌 승";
                chatStruct.color = Color.red;
                ChatManager.Instance.UpdateChat(chatStruct);

                ClickHandler.Instance.isGameSet = true;

                //유저의 스톤과 승자의 스톤이 같다면 true
                Utilities.StartUpdateWinRate(User.Instance.id, User.Instance.stoneColor == pStoneType ? true : false);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
            }
        });
    }
}
