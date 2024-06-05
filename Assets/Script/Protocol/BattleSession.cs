using Google.Protobuf;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

public class BattleSession : Session
{
    private static BattleSession _instance;
    public event Action<ChatStruct> chatRoomRecvEvent;
    public event Action<Protocol.P_Player> quitRoomRecvEvent;
    public event Action<Protocol.P_GameContent> contentRecvEvent;
    public static BattleSession Instance
    {
        get
        {
            if (_instance == null)
            {
                // Scene에서 RoomManager를 찾아 인스턴스화
                _instance = FindObjectOfType<BattleSession>();
                if (_instance == null)
                {
                    // RoomManager가 없는 경우 새로 생성
                    GameObject obj = new GameObject("BattleSession");
                    _instance = obj.AddComponent<BattleSession>();
                }
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        base.Start();

        DontDestroyOnLoad(this);
    }
    // Update is called once per frame
    void Update()
    {

    }

    protected override void HandlePacket(Span<byte> pBuffer, int pLen, ePacketID pID)
    {
        byte[] byteBuffer = pBuffer.ToArray();
        switch (pID)
        {
            case ePacketID.CHAT_MESSAGE:
                Handle_ChatMessage(byteBuffer, pLen);
                break;
            case ePacketID.CONTENT_MESSAGE:
                Handle_ContentMessage(byteBuffer, pLen);
                break;
            //case ePacketID.ENTER_ROOM:
            //    Handle_EnterRoomMessage(byteBuffer, pLen);
            //    break;
            case ePacketID.WINNER_MESSAGE:
                Handle_WinnerMessage(byteBuffer, pLen);
                break;
            case ePacketID.QUIT_ROOM_MESSAGE:
                Handle_QuitRoomMessage(byteBuffer, pLen);
                break;
            case ePacketID.MATCHMAKIING_MESSAGE:
                Handle_MatchmakingMessage(byteBuffer, pLen);
                break;
        }

    }

    unsafe private void Handle_MatchmakingMessage(byte[] pBuffer, int pLen)
    {
        int headerSize = sizeof(PacketHeader);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            LobbyManager.Instance.EnterFastRoom();
        });
    }

    unsafe private void Handle_QuitRoomMessage(byte[] pBuffer, int pLen)
    {
        Debug.Log("Handle_QuitRoomMessage");
        int headerSize = sizeof(PacketHeader);

        Protocol.P_Player pkt = Protocol.P_Player.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            quitRoomRecvEvent(pkt);
        });
    }

    unsafe private void Handle_ChatMessage(byte[] pBuffer, int pLen)
    {
        int headerSize = sizeof(PacketHeader);
        Protocol.S2CChatRoom content = Protocol.S2CChatRoom.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);

        //ReceiveRoomFromServer(room);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            chatRoomRecvEvent(new ChatStruct { name = content.SenderName, content = content.Content });
        });
    }

    unsafe private void Handle_WinnerMessage(byte[] pBuffer, int pLen)
    {
        Debug.Log("Winner: me");

    }

    //unsafe private void Handle_EnterRoomMessage(byte[] pBuffer, int pLen)
    //{
    //    Debug.Log("asd");
    //    int headerSize = sizeof(PacketHeader);
    //    List<Protocol.P_Player> players = new List<Protocol.P_Player>();
    //    Protocol.S2CEnterRoom pkt = Protocol.S2CEnterRoom.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);

    //    for (int i = 0; i < pkt.Players.Count; i += 1)
    //    {
    //        Protocol.P_Player p = pkt.Players[i];
    //        players.Add(p);
    //    }

    //    UnityMainThreadDispatcher.Instance().Enqueue(() =>
    //    {
    //        enterRoomRecvEvent(players);
    //    });

    //}

    unsafe private void Handle_ContentMessage(byte[] pBuffer, int pLen)
    {
        int headerSize = sizeof(PacketHeader);
        Protocol.P_GameContent content = Protocol.P_GameContent.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);

        //ReceiveRoomFromServer(room);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            contentRecvEvent(content);
        });
    }

   
}
