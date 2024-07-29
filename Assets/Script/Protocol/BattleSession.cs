using Google.Protobuf;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
    private GameManagerChecker gameManagerChecker;

    public static BattleSession _instance;
    public event Action<ChatStruct> chatRoomRecvEvent;
    public event Action<Protocol.P_LobbyPlayer> quitRoomRecvEvent;
    public event Action<Protocol.P_GameContent> contentRecvEvent;
    public event Action<List<Protocol.P_LobbyPlayer>> enterRoomRecvEvent;
    public event Action<Protocol.S2CGameStart> enterFastRoomEvent;
    public event Action<Protocol.S2CGameStart> enterCustomRoomEvent;
    public event Action<eStone> gameSetEvent;

    public static BattleSession Instance
    {
        get
        {
            if (_instance == null)
            {
                // Scene에서 RoomManager를 찾아 인스턴스화
                _instance = FindObjectOfType<BattleSession>();
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        base.Start();
        gameManagerChecker = FindObjectOfType<GameManagerChecker>();
        DontDestroyOnLoad(this);
    }

    public async Task RequestEnterFastRoom(int roomId)
    {
        Debug.Log("RequestEnterFastRoom");
        Protocol.C2SEnterRoom pkt = new Protocol.C2SEnterRoom();
        pkt.RoomID = roomId;
        pkt.UserID = User.Instance.id;
        pkt.UserName = User.Instance.userName;
        User.Instance.currentRoomID = roomId;

        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.ENTER_FAST_ROOM);

        UnityMainThreadDispatcher.Instance().Enqueue(async () =>
        {
            await Send(sendBuffer);
        });

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
            case ePacketID.ENTER_ROOM:
                Handle_EnterRoomMessage(byteBuffer, pLen);
                break;
            case ePacketID.WINNER_MESSAGE:
                Handle_WinnerMessage(byteBuffer, pLen);
                break;
            case ePacketID.QUIT_ROOM_MESSAGE:
                Handle_QuitRoomMessage(byteBuffer, pLen);
                break;
            case ePacketID.GAME_START_MESSAGE:
                _ = Handle_StartGame(byteBuffer, pLen).ContinueWith(t =>
                {
                    if (t.Exception != null)
                    {
                        // 예외 처리
                        foreach (var ex in t.Exception.InnerExceptions)
                        {
                            Debug.LogError(ex);
                            Utilities.WriteErrorLog(ex);
                        }
                    }
                }, TaskContinuationOptions.OnlyOnFaulted);
                break;
        }

    }



    private async Task Handle_StartGame(byte[] pBuffer, int pLen)
    {
        Debug.Log("Handle_StartGame");
        int headerSize = 0;
        String userName;
        unsafe
        {
            headerSize = sizeof(PacketHeader);
        }

        Protocol.S2CGameStart pkt = Protocol.S2CGameStart.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);
            
        enterFastRoomEvent(pkt);
        
        
        try
        {
            // 비동기적으로 게임 매니저가 준비될 때까지 대기
            await gameManagerChecker.WaitForGameManagerAsync();

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                try
                {
                    userName = User.Instance.userName;
                    GameManager.Instance.CloseLoadingPanel();
                    GameManager.Instance.isStarted = true;
                }
                catch (Exception e)
                {
                    Debug.LogError(e);
                    Utilities.WriteErrorLog(e);
                }
            });
        }
        catch (Exception e)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                Utilities.WriteErrorLog(e);
            });

            Debug.Log(e.Message);
        }
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
        Debug.Log("players.count: " + players.Count);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            enterRoomRecvEvent(players);
        });
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
        int headerSize = sizeof(PacketHeader);
        Protocol.S2CWinner pkt = Protocol.S2CWinner.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);

        Debug.Log("Winner: " + (eStone)pkt.StoneColor);
        gameSetEvent((eStone)pkt.StoneColor);
    }

    unsafe private void Handle_ContentMessage(byte[] pBuffer, int pLen)
    {
        int headerSize = sizeof(PacketHeader);
        Protocol.P_GameContent content = Protocol.P_GameContent.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            contentRecvEvent(content);
        });
    }

    public void Disconnect()
    {
        if (_client == null)
            return;

        _stream.Close();
        _client.Close();
        _stream = null;
        _client = null;
    }

}
