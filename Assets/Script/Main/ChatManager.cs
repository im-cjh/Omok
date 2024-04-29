using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour
{
    public ScrollRect scrollView;

    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private Transform parentContent; //��ȭ�� ��µǴ� ScrollView�� Content

    [SerializeField]
    private GameObject textChatPrefab; //��ȭ�� ����ϴ� Text UI ������

    private string ID = "Docker"; //�ӽ� ���̵�

    public void OnEndEditEventMethod()
    {
        //enterŰ�� ������ ��ȭ �Է�â�� �Էµ� ������ ��ȭâ�� ��� 
        if(Input.GetKeyDown(KeyCode.Return))
        {
            UpdateChat();
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            UpdateChat();
        }
        
    }

    public void UpdateChat()
    {
        if (inputField.text.Equals(""))
            return;

        //��ȭ ���� ����� ���� Text UI ���� 
        GameObject clone = Instantiate(textChatPrefab, parentContent);

        //��ȭ �Է�â�� �ִ� ������ ��ȭâ�� ���(ID: ����) 
        clone.GetComponent<TextMeshProUGUI>().text = $"{ID}: {inputField.text}";

        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;

        //��ȭ �Է�â�� �ִ� ���� �ʱ�ȭ 
        inputField.text = "";
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            //��ȭ �Է�â�� ��Ŀ�� �Ǿ����� ���� �� EnterŰ�� ������
            if (inputField.isFocused == false)
            {
                //��ȭ �Է�â�� ��Ŀ���� Ȱ��ȭ 
                inputField.ActivateInputField();
            }
            else
            {
                //��ȭ ���� ����

            }
        }
    }
}
