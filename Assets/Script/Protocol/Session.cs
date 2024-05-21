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


public class DataSentEventArgs : EventArgs
{
    public Room Data { get; }

    public DataSentEventArgs(Room data)
    {
        Data = data;
    }
}

public class Session : MonoBehaviour
{
    private static Session _instance;

    public event Action<Dictionary<int, Room>> roomRecvEvent;
    public event Action<Protocol.P_GameContent> contentRecvEvent; 
    public event Action<List<Protocol.P_Player>> enterRoomRecvEvent;
    public event Action<Protocol.P_Player> quitRoomRecvEvent;
    public event Action<ChatStruct> chatRoomRecvEvent;

    public User _user;
    private string serverAddress = "127.0.0.1"; // 서버 IP 주소
    private int serverPort = 7777; // 서버 포트 번호

    private TcpClient _client = new TcpClient();
    private NetworkStream _stream;
    private bool _isConnected = false;
    private byte[] _recvBuffer = new byte[1024];

    public static Session Instance
    {
        get
        {
            if (_instance == null)
            {
                // Scene에서 RoomManager를 찾아 인스턴스화
                _instance = FindObjectOfType<Session>();
                if (_instance == null)
                {
                    // RoomManager가 없는 경우 새로 생성
                    GameObject obj = new GameObject("session");
                    _instance = obj.AddComponent<Session>();
                }
            }
            return _instance;
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        _client = new TcpClient();
        _user = FindObjectOfType<User>();

        DontDestroyOnLoad(this);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandlePacket(Span<byte> pBuffer, int pLen, ePacketID pID)
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
        }

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
            chatRoomRecvEvent(new ChatStruct{name =  content.SenderName, content = content.Content });
        });
    }

    unsafe private void Handle_WinnerMessage(byte[] pBuffer, int pLen)
    {
        Debug.Log("Winner: me");
        
    }

    unsafe private void Handle_EnterRoomMessage(byte[] pBuffer, int pLen)
    {
        int headerSize = sizeof(PacketHeader);
        List<Protocol.P_Player> players = new List<Protocol.P_Player>();
        Protocol.S2CEnterRoom pkt = Protocol.S2CEnterRoom.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);

        for (int i = 0; i < pkt.Players.Count; i += 1)
        {
            Protocol.P_Player p = pkt.Players[i];
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
        Protocol.P_GameContent content  = Protocol.P_GameContent.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);
        
        //ReceiveRoomFromServer(room);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            contentRecvEvent(content);
        });
    }

    unsafe void Handle_RoomsMessage(byte[] buffer, int len)
    {
        
        int headerSize = sizeof(PacketHeader);
        Protocol.S2CRoomList rooms = Protocol.S2CRoomList.Parser.ParseFrom(buffer, headerSize, len - headerSize);
        Dictionary<int, Room> roomList = new Dictionary<int, Room>(); 
        for(int i = 0; i < rooms.Rooms.Count; i += 1)
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

    private unsafe void RecvCallback(IAsyncResult result)
    {
        int bytesRead = _stream.EndRead(result); // 데이터 수신을 완료합니다.
        try
        {
            if (bytesRead > 0)
            {
                fixed (byte* ptr = _recvBuffer)
                {
                    PacketHeader* header = (PacketHeader*)(ptr);
                    // 헤더에 기록된 패킷 크기를 파싱할 수 있어야 한다
                    if (bytesRead < header->size)
                    {
                        Debug.Log("me");
                        return;
                    }
                    HandlePacket(_recvBuffer, header->size, (ePacketID)header->id);
                }

                // 다시 비동기적으로 데이터 수신을 시작합니다.
                _stream.BeginRead(_recvBuffer, 0, _recvBuffer.Length, RecvCallback, null);
            }
            else
            {
                // 연결이 종료된 경우
                Debug.Log("Connection closed by server.");
                _client.Close();
            }
        }
        catch(Exception e) 
        {
            Debug.Log(e);
            _stream.BeginRead(_recvBuffer, 0, _recvBuffer.Length, RecvCallback, null);
        }
    }

    public async Task Connect()
    {
        try
        {
            await _client.ConnectAsync(serverAddress, serverPort);
            
            _stream = _client.GetStream();
            Console.WriteLine("Connected to server!");
            _stream.BeginRead(_recvBuffer, 0, _recvBuffer.Length, RecvCallback, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to connect: " + ex.Message);
            throw ex;
        }
    }

    public async Task Send(byte[] pSendBuffer)
    {
        await _stream.WriteAsync(pSendBuffer, 0, pSendBuffer.Length);
    }
}
