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

public abstract class Session : MonoBehaviour
{
    public User _user;

    protected byte[] _recvBuffer = new byte[1024];

    private bool _isConnected = false;
    protected NetworkStream _stream;
    protected TcpClient _client = new TcpClient();

    // Start is called before the first frame update
    protected void Start()
    {
        _client = new TcpClient();
        _user = FindObjectOfType<User>();
    }

    protected abstract void HandlePacket(Span<byte> pBuffer, int pLen, ePacketID pID);

    protected unsafe void RecvCallback(IAsyncResult result)
    {
        int bytesRead = _stream.EndRead(result); // ������ ������ �Ϸ��մϴ�.
        try
        {
            if (bytesRead > 0)
            {
                fixed (byte* ptr = _recvBuffer)
                {
                    PacketHeader* header = (PacketHeader*)(ptr);
                    // ����� ��ϵ� ��Ŷ ũ�⸦ �Ľ��� �� �־�� �Ѵ�
                    if (bytesRead < header->size)
                    {
                        Debug.Log("me");
                        return;
                    }
                    HandlePacket(_recvBuffer, header->size, (ePacketID)header->id);
                }

                // �ٽ� �񵿱������� ������ ������ �����մϴ�.
                _stream.BeginRead(_recvBuffer, 0, _recvBuffer.Length, RecvCallback, null);
            }
            else
            {
                // ������ ����� ���
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

    public void Connect(string pIP, int pPort)
    {
        try
        {
            _client.Connect(pIP, pPort);
            
            _stream = _client.GetStream();
            Debug.Log("Connected to server!");

            _stream.BeginRead(_recvBuffer, 0, _recvBuffer.Length, RecvCallback, null);
        }
        catch (Exception ex)
        {
            Debug.Log("Failed to connect: " + ex.Message);
            throw ex;
        }
    }

    public async Task Send(byte[] sendBuffer)
    {
        if (sendBuffer == null || sendBuffer.Length == 0)
        {
            Debug.Log("Send buffer is null or empty");
            return;
        }

        if (_stream == null)
        {
            Debug.Log("networkStream is null");
            return;
        }

        

        // ���� ��Ʈ��ũ ���� �ڵ�
        try
        {
            // ��Ʈ��ũ ��Ʈ���� ���� ������ ����
            await _stream.WriteAsync(sendBuffer, 0, sendBuffer.Length);
            
        }
        catch (Exception e)
        {
            Debug.Log("Exception in Send: " + e.Message);
        }
    }

}
