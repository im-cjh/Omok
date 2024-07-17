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

public class LobbySession : Session
{
    private static LobbySession _instance;

    public event Action<Dictionary<int, Room>> roomRecvEvent;
    public event Action<Protocol.P_GameContent> contentRecvEvent;
    public event Action<List<Protocol.P_LobbyPlayer>> enterRoomRecvEvent;
    public event Action<Protocol.P_LobbyPlayer> quitRoomRecvEvent;
    public event Action<ChatStruct> chatRoomRecvEvent;

    public static LobbySession Instance
    {
        get
        {
            if (_instance == null)
            {
                // Scene에서 RoomManager를 찾아 인스턴스화
                _instance = FindObjectOfType<LobbySession>();
                //if (_instance == null)
                //{
                //    // RoomManager가 없는 경우 새로 생성
                //    GameObject obj = new GameObject("lobbySession");
                //    _instance = obj.AddComponent<LobbySession>();
                //}
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

    protected override void HandlePacket(Span<byte> pBuffer, int pLen, ePacketID pID)
    {
        byte[] byteBuffer = pBuffer.ToArray();
        switch (pID)
        {
            case ePacketID.CHAT_MESSAGE:
                Handle_ChatMessage(byteBuffer, pLen);
                break;
            case ePacketID.ROOMS_MESSAGE:
                Handle_RoomsMessage(byteBuffer, pLen);
                break;
            case ePacketID.CONTENT_MESSAGE:
                Handle_ContentMessage(byteBuffer, pLen);
                break;
            case ePacketID.ENTER_ROOM:
                Handle_EnterRoomMessage(byteBuffer, pLen);
                break;
            case ePacketID.WINNER_MESSAGE:
                Handle_WinnerMessage(byteBuffer, pLen);
                break;
            case ePacketID.QUIT_ROOM_MESSAGE:
                Handle_QuitRoomMessage(byteBuffer, pLen);
                break;
            case ePacketID.MATCHMAKED_MESSAGE:
                Handle_MatchmakingMessageAsync(byteBuffer, pLen);
                break;
        }
    }

    private void Handle_MatchmakingMessageAsync(byte[] pBuffer, int pLen)
    {
        try
        {
            int headerSize = Marshal.SizeOf<PacketHeader>();
            Debug.Log("Handle_MatchmakingMessage");
            Protocol.S2CBattleServerAddr pkt = Protocol.S2CBattleServerAddr.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);
            
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                LobbyManager.Instance.RoomID = pkt.RoomID;
                BattleSession.Instance.Connect(pkt.BattleServerIp, pkt.Port);
                BattleSession.Instance.RequestEnterFastRoom(pkt.RoomID);
            });
            
        }
        catch (Exception e)
        {
            Debug.LogError("Exception in Handle_MatchmakingMessageAsync: " + e.Message);
        }
    }


    unsafe private void Handle_QuitRoomMessage(byte[] pBuffer, int pLen)
    {
        Debug.Log("Handle_QuitRoomMessage");
        int headerSize = sizeof(PacketHeader);
        

        Protocol.P_LobbyPlayer pkt = Protocol.P_LobbyPlayer.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            quitRoomRecvEvent(pkt);
        });
    }

    unsafe private void Handle_ChatMessage(byte[] pBuffer, int pLen)
    {
        Debug.Log("Handle_ChatMessage");
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

    unsafe private void Handle_EnterRoomMessage(byte[] pBuffer, int pLen)
    {
        Debug.Log("Handle_EnterRoomMessage");
        int headerSize = sizeof(PacketHeader);
        List<Protocol.P_LobbyPlayer> players = new List<Protocol.P_LobbyPlayer>();
        Protocol.S2CEnterRoom pkt = Protocol.S2CEnterRoom.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);

        for (int i = 0; i < pkt.Players.Count; i += 1)
        {
            Protocol.P_LobbyPlayer p = pkt.Players[i];
            players.Add(p);
        }

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            enterRoomRecvEvent(players);
        });
    }

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

    unsafe void Handle_RoomsMessage(byte[] buffer, int len)
    {
        Debug.Log("Handle_RoomsMessage");
        int headerSize = sizeof(PacketHeader);
        Protocol.S2CRoomList rooms = Protocol.S2CRoomList.Parser.ParseFrom(buffer, headerSize, len - headerSize);
        Dictionary<int, Room> roomList = new Dictionary<int, Room>();
        for (int i = 0; i < rooms.Rooms.Count; i += 1)
        {
            Protocol.P_Room r = rooms.Rooms[i];
            Room room = new Room();
            room.roomId = r.RoomID;
            room.roomName = r.RoomName;
            room.hostName = r.HostName;
            room.numParticipants = r.NumPlayers;
            roomList[room.roomId] = room;
        }


        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            roomRecvEvent(roomList);
        });
    }
}
