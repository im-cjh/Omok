using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEditor.Experimental.GraphView;
using UnityEditor.PackageManager;
using UnityEngine;

public class User : MonoBehaviour
{
    public string user_name;

    private TcpClient client;
    private NetworkStream stream;

    private string serverAddress = "127.0.0.1"; // ���� IP �ּ�
    private int serverPort = 7777; // ���� ��Ʈ ��ȣ

    // Start is called before the first frame update
    void Start()
    {
        DontDestroyOnLoad(this);
    }

    public async Task ConnectToServer()
    {
        client = new TcpClient();

        try
        {
            await client.ConnectAsync(serverAddress, serverPort);
            Console.WriteLine("Connected to server!");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Failed to connect: " + ex.Message);
            throw ex; 
        }
        finally
        {
            //client.Close();
        }
    }
}
