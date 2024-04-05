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

        // 일정 시간 후에 메시지를 자동으로 닫을 수 있도록 예약
        Invoke("HideMessage", 5f);
    }

    public void HideMessage()
    {
        messagePanel.SetActive(false);
    }
}
