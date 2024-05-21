using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public GameObject messagePanel;
    public Text messageText;

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
}
