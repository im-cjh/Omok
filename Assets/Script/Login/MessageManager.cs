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

            }
            return _instance;
        }
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
