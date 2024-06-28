using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MessageManager : MonoBehaviour
{
    public GameObject messagePanel;
    public GameObject signUpPanel;
    public TMP_Text messageText;

    public void ShowMessage(string message)
    {
        try
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                messageText.text = message;
                messagePanel.SetActive(true);
                // 일정 시간 후에 메시지를 자동으로 닫을 수 있도록 예약
                Invoke("HideMessage", 5f);
            });
        }
        catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }
    }

    public void HideMessage()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            messagePanel.SetActive(false);
            // 일정 시간 후에 메시지를 자동으로 닫을 수 있도록 예약
            Invoke("HideMessage", 5f);
        });
    }

    public void CloseSignUpPanel()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            signUpPanel.SetActive(false);
            // 일정 시간 후에 메시지를 자동으로 닫을 수 있도록 예약
            Invoke("HideMessage", 5f);
        });
    }

}
