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

    private string _name; //�ӽ� ���̵�

    private void Start()
    {
        _name = User.Instance.userName;
    }
    public void OnEndEditEventMethod()
    {
        //enterŰ�� ������ ��ȭ �Է�â�� �Էµ� ������ ��ȭâ�� ��� 
        if(Input.GetKeyDown(KeyCode.Return))
        {
            UpdateChat(_name, inputField.text);
        }

        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            UpdateChat(_name, inputField.text);
        }
        
    }

    public void UpdateChat(string pName, string pContent)
    {
        if (inputField.text.Equals(""))
            return;

        //��ȭ ���� ����� ���� Text UI ���� 
        GameObject clone = Instantiate(textChatPrefab, parentContent);

        //��ȭ �Է�â�� �ִ� ������ ��ȭâ�� ���(ID: ����) 
        clone.GetComponent<TextMeshProUGUI>().text = $"{pName}: {pContent}";

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
