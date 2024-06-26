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
                // Scene���� MessageManager ã�� �ν��Ͻ�ȭ
                _instance = FindObjectOfType<MessageManager>();
                if (_instance == null)
                {
                    // RoomManager�� ���� ��� ���� ����
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

        // ���� �ð� �Ŀ� �޽����� �ڵ����� ���� �� �ֵ��� ����
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
