using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    private GameObject textChatPrefab; //대화를 출력하는 Text UI 프리팹

    private User _user;
    private Session _session;

    private void Start()
    {
        try
        {
            _user = FindObjectOfType<User>(); 
            _session = FindObjectOfType<Session>();
            _session.onRoomReceived += OnRecvRoom;

            ReloadRoom();
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
            //대화 입력창에 있는 내용을 대화창에 출력(ID: 내용) 
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
        
        Task.Run(async () =>
        {
            byte[] sendBuffer = PacketHandler.MakeMemoryStream(ePacketID.ROOMS_MESSAGE).ToArray();
            await _session.Send(sendBuffer);
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
}
