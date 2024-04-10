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
    public void OnRecvRoom(List<Room> room)
    {
        try
        {
            //��ȭ �Է�â�� �ִ� ������ ��ȭâ�� ���(ID: ����) 
            foreach(Room r in room)
            {
                GameObject roomObject = Instantiate(textChatPrefab, _transform);
                RoomUI roomUI = roomObject.GetComponent<RoomUI>();
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
