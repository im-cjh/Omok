using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public ScrollRect scrollView;

    [SerializeField]
    private Transform _transform; 

    [SerializeField]
    private GameObject textChatPrefab; //��ȭ�� ����ϴ� Text UI ������

    public void OnEndEditEventMethod()
    {
            UpdateChat();
    }

    public void ReloadRoom()
    {
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
