using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using static System.Collections.Specialized.BitVector32;

public class LobbyManager : MonoBehaviour
{
    private static LobbyManager _instance;

    private ScrollRect _scrollView;

    private Transform _transform;

    private GameObject roomPrefab; //대화를 출력하는 Text UI 프리팹

    private Room _selectedRoom;

    private Dictionary<int, Room> _rooms;
    public int RoomID;

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
                    GameObject obj = new GameObject("Lobby Manager");
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
            LobbySession.Instance.roomRecvEvent += Instance.OnRecvRoom;
            ReloadRoom();
            DontDestroyOnLoad(this);
        }
        catch (Exception e)
        {
            return;
        }
    }

    public void OnSceneChanged(ScrollRect pScrollView)
    {
        _transform = pScrollView.viewport.GetChild(0);
        roomPrefab = Resources.Load<GameObject>("Room");
        _scrollView = pScrollView;

        ReloadRoom();
    }

    public void OnClickedFastGame()
    {
        try
        {
            Task.Run(async () =>
            {
                byte[] sendBuffer = PacketHandler.SerializeHeader(ePacketID.MATCHMAKIING_MESSAGE);
                await LobbySession.Instance.Send(sendBuffer);
            });
        }
        catch (Exception ex)
        {
            Utilities.WriteErrorLog(ex);
        }
    }

    public void EnterFastRoom()
    {
        SceneChanger.ChangeGameScene();
    }

    public void EnterCustomRoom()
    {
        if (_selectedRoom == null)
            return;

        SceneChanger.ChangeGameScene();

        Protocol.C2SEnterRoom pkt = new Protocol.C2SEnterRoom();
        pkt.RoomID = _selectedRoom.roomId;
        pkt.UserID = LobbySession.Instance._user.id;
        pkt.Win = User.Instance.userWin;
        pkt.Lose = User.Instance.userLose;

        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.ENTER_ROOM);

        Task.Run(async () =>
        {
            await LobbySession.Instance.Send(sendBuffer);
        });

    }

    public void OnClickedRoom(Room pRoom)
    {
        _selectedRoom = pRoom;

    }

    public void OnClickedMakeRoom(Room pRoom)
    {
        Debug.Log("OnClickedMakeRoom");

        Protocol.C2SMakeRoom pkt = new Protocol.C2SMakeRoom();
        //pkt.
        //LobbySession.Instance.Send();
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
                GameObject roomObject = Instantiate(roomPrefab, _transform);
                RoomUI roomUI = roomObject.GetComponent<RoomUI>();
                roomUI.SetRoomInfo(room.Value);
            }
            
            Canvas.ForceUpdateCanvases();
            _scrollView.verticalNormalizedPosition = 0f;
        }
        catch(Exception e) 
        {
            Debug.Log("OnRecvRoom's error Called" + e.Message);
            
            return;
        }
    }
    
    public void OnEndEditEventMethod()
    {
            UpdateChat();
    }



    public void ReloadRoom()
    {
        Task.Run(async () =>
        {
            byte[] sendBuffer = PacketHandler.SerializeHeader(ePacketID.ROOMS_MESSAGE);
            await LobbySession.Instance.Send(sendBuffer);
        });
    }

    public void UpdateChat()
    {
        //대화 내용 출력을 위해 Text UI 생성 
        GameObject clone = Instantiate(roomPrefab, _transform);

        //대화 입력창에 있는 내용을 대화창에 출력(ID: 내용) 
        //clone.GetComponent<TextMeshProUGUI>().text = $"{ID}: {inputField.text}";

        Canvas.ForceUpdateCanvases();
        _scrollView.verticalNormalizedPosition = 0f;
    }

    public Room GetSelectedRoom()
    {
        return _selectedRoom;
    }
}
