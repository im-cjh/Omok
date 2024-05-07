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
    public event Action<Dictionary<int, Room>> roomRecvEvent;
    public event Action<Protocol.P_GameContent> contentRecvEvent; 
    public event Action<List<Protocol.P_Player>> enterRoomRecvEvent;

    public User _user;
    private string serverAddress = "127.0.0.1"; // 서버 IP 주소
    private int serverPort = 7777; // 서버 포트 번호

    private TcpClient _client = new TcpClient();
    private NetworkStream _stream;
    private bool _isConnected = false;
    private byte[] _recvBuffer = new byte[1024];
    

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

    private void HandlePacket(Span<byte> buffer, int len, ePacketID ID)
    {
        Debug.Log("HandlePacket called");
        byte[] byteBuffer = buffer.ToArray();
        switch (ID)
        {
            case ePacketID.ROOMS_MESSAGE:
                Handle_RoomsMessage(byteBuffer, len);
                break;
            case ePacketID.CONTENT_MESSAGE:
                Handle_ContentMessage(byteBuffer, len); 
                break;
            case ePacketID.ENTER_ROOM:
                Handle_EnterRoomMessage(byteBuffer, len);
                break;
            
        }

    }

    unsafe private void Handle_EnterRoomMessage(byte[] pBuffer, int pLen)
    {
        Debug.Log("Enter Room called");
        int headerSize = sizeof(PacketHeader);
        List<Protocol.P_Player> players = new List<Protocol.P_Player>();
        Protocol.S2CEnterRoom pkt = Protocol.S2CEnterRoom.Parser.ParseFrom(pBuffer, headerSize, pLen - headerSize);

        for (int i = 0; i < pkt.Players.Count; i += 1)
        {
            Protocol.P_Player p = pkt.Players[i];
            players.Add(p);
            Debug.Log(p.UserName);
        }
        
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            //Debug.Log(players.Count);

            if (players != null)
            {
                Debug.Log("Players.count = " + players.Count);
               enterRoomRecvEvent(players);
            }
            else
            {
                Debug.Log("Sibal jabatDDa");

            }
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

    //public void RecvRoomFromServer(List<Room> rooms)
    //{
    //    // 받아온 Room 데이터를 이벤트로 전달
    //    roomRecvEvent.Invoke(rooms);
    //}

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
        Debug.Log("Im called");
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
                        Debug.Log("설마 너냐");
                        Debug.Log(bytesRead + " : " + header->size + (ePacketID)header->id);
                        return;
                    }
                    if(bytesRead > header->size)
                    {
                        Debug.Log("A");
                    }
                    else if (bytesRead == header->size)
                    {
                        Debug.Log("B");
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
