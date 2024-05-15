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
    private GameObject textChatPrefab; //대화를 출력하는 Text UI 프리팹
    private Room _selectedRoom;
    private Dictionary<int, Room> _rooms;

    public static LobbyManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Scene에서 RoomManager를 찾아 인스턴스화
                _instance = FindObjectOfType<LobbyManager>();
                if (_instance == null)
                {
                    // RoomManager가 없는 경우 새로 생성
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
            //_session = FindObjectOfType<Session>();
            Session.Instance.roomRecvEvent += OnRecvRoom;
            ReloadRoom();
            DontDestroyOnLoad(this);
        }
        catch (Exception e)
        {
            return;
        }
    }

    public void OnSceneChanged()
    {
        Debug.Log("OnSceneChanged");
        GameObject contentGO = GameObject.Find("content");
        _transform = contentGO.transform;
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
        pkt.UserID = Session.Instance._user.id;
        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.ENTER_ROOM);
        Session.Instance.Send(sendBuffer);

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
            await Session.Instance.Send(sendBuffer);
        });
    }

    public void UpdateChat()
    {
        //대화 내용 출력을 위해 Text UI 생성 
        GameObject clone = Instantiate(textChatPrefab, _transform);

        //대화 입력창에 있는 내용을 대화창에 출력(ID: 내용) 
        //clone.GetComponent<TextMeshProUGUI>().text = $"{ID}: {inputField.text}";

        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;
    }

    public Room GetSelectedRoom()
    {
        return _selectedRoom;
    }
}
