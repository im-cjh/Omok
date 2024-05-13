using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager _instance;

    public ScrollRect scrollView;

    [SerializeField]
    private Transform _transform;

    [SerializeField]
    private GameObject textChatPrefab; //��ȭ�� ����ϴ� Text UI ������

    private Session _session;
    private Room _selectedRoom;
    private Dictionary<int, Room> _rooms;

    public static LobbyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Scene���� RoomManager�� ã�� �ν��Ͻ�ȭ
                _instance = FindObjectOfType<LobbyManager>();
                if (_instance == null)
                {
                    // RoomManager�� ���� ��� ���� ����
                    GameObject obj = new GameObject("LobbyManager");
                    _instance = obj.AddComponent<LobbyManager>();
                }
            }
            return _instance;
        }
    }

    private void Start()
    {
        try
        {
            _session = FindObjectOfType<Session>();
            _session.roomRecvEvent += OnRecvRoom;
            ReloadRoom();
            DontDestroyOnLoad(this);
        }
        catch (Exception e)
        {
            return;
        }
    }

    public void FastGameClick()
    {

    }

    public void EnterRoom()
    {
        if (_selectedRoom == null)
            return;

        SceneChanger.ChangeGameScene();

        Protocol.C2SEnterRoom pkt = new Protocol.C2SEnterRoom();
        pkt.RoomID = _selectedRoom.roomId;
        pkt.UserID = _session._user.id;
        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.ENTER_ROOM);
        _session.Send(sendBuffer);

    }

    public void OnClickedRoom(Room pRoom)
    {
        _selectedRoom = pRoom;
        
    }

    public void OnRecvRoom(Dictionary<int, Room> rooms)
    {
        
        _rooms = rooms;
        foreach (Transform child in _transform)
        {
            Destroy(child.gameObject);
        }

        try
        {
            foreach(var room in _rooms)
            {
                GameObject roomObject = Instantiate(textChatPrefab, _transform);
                RoomUI roomUI = roomObject.GetComponent<RoomUI>();
                roomUI.SetRoomInfo(room.Value);
            }
            
            Canvas.ForceUpdateCanvases();
            scrollView.verticalNormalizedPosition = 0f;
        }
        catch(Exception e) 
        {
            Debug.Log("OnRecvRoom's error Called");
            return;
        }
    }
    
    public void OnEndEditEventMethod()
    {
            UpdateChat();
    }



    public void ReloadRoom()
    {
        //byte[] sendBuffer = PacketHandler.SerializeHeader(ePacketID.ROOMS_MESSAGE);
        //_session.Send(sendBuffer);
        Task.Run(async () =>
        {
            byte[] sendBuffer = PacketHandler.SerializeHeader(ePacketID.ROOMS_MESSAGE);
            await _session.Send(sendBuffer);
        });
    }

    public void UpdateChat()
    {
        //��ȭ ���� ����� ���� Text UI ���� 
        GameObject clone = Instantiate(textChatPrefab, _transform);

        //��ȭ �Է�â�� �ִ� ������ ��ȭâ�� ���(ID: ����) 
        //clone.GetComponent<TextMeshProUGUI>().text = $"{ID}: {inputField.text}";

        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;
    }

    public Room GetSelectedRoom()
    {
        return _selectedRoom;
    }
}
