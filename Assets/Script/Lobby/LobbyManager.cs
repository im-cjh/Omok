using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static System.Collections.Specialized.BitVector32;

public class LobbyManager : MonoBehaviour
{
    public ScrollRect scrollView;

    [SerializeField]
    private Transform _transform; 

    [SerializeField]
    private GameObject textChatPrefab; //��ȭ�� ����ϴ� Text UI ������

    private User _user;
    private Session _session;

    private void Start()
    {
        try
        {
            _user = FindObjectOfType<User>(); 
            _session = FindObjectOfType<Session>();
            _session.onRoomReceived += OnRecvRoom;

            _session.Request(eReqType.Rooms);
        }
        catch (Exception e) 
        {
            return;
        }
    }
    public void OnRecvRoom(Room room)
    {
        try
        {
            GameObject roomObject = Instantiate(textChatPrefab, _transform);
            RoomUI roomUI = roomObject.GetComponent<RoomUI>();
            //��ȭ �Է�â�� �ִ� ������ ��ȭâ�� ���(ID: ����) 
            if (roomUI != null)
            {
                Room r = new Room();
                r.roomName = room.roomName;
                r.hostName = room.hostName;
                r.numParticipants = room.numParticipants;
                roomUI.SetRoomInfo(r);
            }
            Canvas.ForceUpdateCanvases();
            scrollView.verticalNormalizedPosition = 0f;
        }
        catch(Exception e) 
        {
            return;
        }
    }
    public void OnEndEditEventMethod()
    {
            UpdateChat();
    }

    public void ReloadRoom()
    {
        _session.Request(eReqType.Rooms);

        //��ȭ ���� ����� ���� Text UI ���� 
        GameObject roomObject = Instantiate(textChatPrefab, _transform);
        RoomUI roomUI = roomObject.GetComponent<RoomUI>();
        //��ȭ �Է�â�� �ִ� ������ ��ȭâ�� ���(ID: ����) 
        if (roomUI != null)
        {
            Room r = new Room();
            r.roomName = "1";
            r.hostName = "2";
            r.numParticipants = 3;
            roomUI.SetRoomInfo(r);
        }
        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;
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
}
