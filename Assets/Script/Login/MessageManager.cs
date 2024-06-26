using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public static MessageManager _instance;

    public GameObject messagePanel;
    public GameObject signUpPanel;
    public TMP_Text messageText;

    public static MessageManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Scene에서 MessageManager 찾아 인스턴스화
                _instance = FindObjectOfType<MessageManager>();
                if (_instance == null)
                {
                    // RoomManager가 없는 경우 새로 생성
                    GameObject obj = new GameObject("MessageManager");
                    _instance = obj.AddComponent<MessageManager>();
                }
            }
            return _instance;
        }
    }

    private void Start()
    {
        messagePanel = GameObject.Find("Canvas/MessagePanel");
        messageText = Utilities.FindAndAssign<TMP_Text>("Canvas/MessagePanel/messageText");
    }

    public void ShowMessage(string message)
    {
        messageText.text = message;
        messagePanel.SetActive(true);

        // 일정 시간 후에 메시지를 자동으로 닫을 수 있도록 예약
        Invoke("HideMessage", 5f);
    }

    public void HideMessage()
    {
        messagePanel.SetActive(false);
    }

    public void CloseSignUpPanel()
    {
        signUpPanel.SetActive(false);
    }

}
