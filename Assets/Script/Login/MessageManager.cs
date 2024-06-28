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
                // ���� �ð� �Ŀ� �޽����� �ڵ����� ���� �� �ֵ��� ����
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
            // ���� �ð� �Ŀ� �޽����� �ڵ����� ���� �� �ֵ��� ����
            Invoke("HideMessage", 5f);
        });
    }

    public void CloseSignUpPanel()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            signUpPanel.SetActive(false);
            // ���� �ð� �Ŀ� �޽����� �ڵ����� ���� �� �ֵ��� ����
            Invoke("HideMessage", 5f);
        });
    }

}
