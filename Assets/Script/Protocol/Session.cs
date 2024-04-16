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
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.Events;

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
    public event Action<List<Room>> onRoomReceived;
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
        DontDestroyOnLoad(this);
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    private void HandlePacket(Span<byte> buffer, int len, ePacketID ID)
    {
        byte[] byteBuffer = buffer.ToArray();
        switch (ID)
        {
            case ePacketID.ROOMS_MESSAGE:
                Handle_RoomsMessage(byteBuffer, len);
                break;
        }

    }

    public void ReceiveRoomFromServer(List<Room> rooms)
    {
        // 받아온 Room 데이터를 이벤트로 전달
        onRoomReceived.Invoke(rooms);
    }

    

    unsafe void Handle_RoomsMessage(byte[] buffer, int len)
    {
        int headerSize = sizeof(PacketHeader);
        Protocol.S2CRoomList rooms = Protocol.S2CRoomList.Parser.ParseFrom(buffer, headerSize, len - headerSize);
        List<Room> roomList = new List<Room>(); 
        for(int i = 0; i < rooms.Rooms.Count; i += 1)
        {
            Protocol.P_Room r = rooms.Rooms[i];
            Room room = new Room();
            room.roomName = r.RoomName;
            room.hostName = r.HostName;
            room.numParticipants = r.NumPlayers;
            roomList.Add(room);
        }

        //ReceiveRoomFromServer(room);
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            onRoomReceived(roomList);
        });
    }

    private unsafe void ReceiveCallback(IAsyncResult result)
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
                        return;

                    HandlePacket(_recvBuffer, header->size, ePacketID.ROOMS_MESSAGE);
                }

                // 다시 비동기적으로 데이터 수신을 시작합니다.
                _stream.BeginRead(_recvBuffer, 0, _recvBuffer.Length, ReceiveCallback, null);
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
            _stream.BeginRead(_recvBuffer, 0, _recvBuffer.Length, ReceiveCallback, null);
        }
    }

    public async Task Connect()
    {
        try
        {
            await _client.ConnectAsync(serverAddress, serverPort);
            
            _stream = _client.GetStream();
            Console.WriteLine("Connected to server!");
            _stream.BeginRead(_recvBuffer, 0, _recvBuffer.Length, ReceiveCallback, null);
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to connect: " + ex.Message);
            throw ex;
        }
    }

    public async Task Send<T>(ePacketID pMessageID, T pSendBuffer = default)
    {
        switch(pMessageID) 
        {
            case ePacketID.ROOMS_MESSAGE:
                await RequestRooms();
                break;

            case ePacketID.CONTENT_MESSAGE:
                if (pSendBuffer is Vector2)
                {
                    Vector2 sendBuffer = (Vector2)(object)pSendBuffer;
                    await SendContent(sendBuffer);
                }
                break;

            default:
                break;
        }
    }
    
    private async Task RequestRooms()
    {
        try
        {
            PacketHeader packetHeader = new PacketHeader();
            packetHeader.size = (UInt16)Marshal.SizeOf(typeof(PacketHeader));
            packetHeader.id     = (UInt16)ePacketID.ROOMS_MESSAGE;

            using (var memoryStream = new MemoryStream())
            {
                // PacketHeader를 바이트 배열로 변환하여 MemoryStream에 쓰기
                using (var writer = new BinaryWriter(memoryStream, Encoding.Default, true))
                {
                    writer.Write(packetHeader.size);
                    writer.Write(packetHeader.id);
                }

                // 최종 바이트 배열 가져오기
                byte[] finalBytes = memoryStream.ToArray();
                
                await _stream.WriteAsync(finalBytes, 0, finalBytes.Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while sending data: " + e.Message);
        }
    }

    private async Task SendContent(Int32 roomID, Vector2 pPos)
    {
        Debug.Log(3);
        try
        {
            Protocol.P_GameContent pkt = new Protocol.P_GameContent();
            pkt.YPos = pPos.y  ;

            PacketHeader packetHeader = new PacketHeader();
            packetHeader.size = (UInt16)Marshal.SizeOf(typeof(PacketHeader));
            packetHeader.id = (UInt16)ePacketID.CONTENT_MESSAGE;

            using (var memoryStream = new MemoryStream())
            {
                // PacketHeader를 바이트 배열로 변환하여 MemoryStream에 쓰기
                using (var writer = new BinaryWriter(memoryStream, Encoding.Default, true))
                {
                    writer.Write(packetHeader.size);
                    writer.Write(packetHeader.id);
                }
                
                // 최종 바이트 배열 가져오기
                byte[] finalBytes = memoryStream.ToArray();

                Debug.Log(_recvBuffer);
                await _stream.WriteAsync(finalBytes, 0, finalBytes.Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while sending data: " + e.Message);
        }
    }
}
