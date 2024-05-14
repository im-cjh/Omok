using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class  ChatStruct
{
    public string name;
    public string content;
}

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
        Session.Instance.chatRoomRecvEvent += UpdateChat;
    }
    public void OnEndEditEventMethod()
    {
        //enterŰ�� ������ ��ȭ �Է�â�� �Էµ� ������ ��ȭâ�� ��� 
        if (Input.GetKeyDown(KeyCode.Return))
        {
            UpdateChat(new ChatStruct { name = _name, content = inputField.text });
        }

        else if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            UpdateChat(new ChatStruct { name = _name, content = inputField.text });
        }
    }

    public void UpdateChat(ChatStruct pChat)
    {
        Debug.Log("asdas");
        if (pChat.content.Equals(""))
            return;

        //��ȭ ���� ����� ���� Text UI ���� 
        GameObject clone = Instantiate(textChatPrefab, parentContent);

        //��ȭ �Է�â�� �ִ� ������ ��ȭâ�� ���(ID: ����) 
        clone.GetComponent<TextMeshProUGUI>().text = $"{pChat.name}: {pChat.content}";

        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;
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
                Protocol.C2SChatRoom pkt = new Protocol.C2SChatRoom();
                pkt.SenderName = _name;
                pkt.Content = inputField.text;

                byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.CHAT_MESSAGE);
                Session.Instance.Send(sendBuffer);
                //��ȭ �Է�â�� �ִ� ���� �ʱ�ȭ 
                inputField.text = "";
            }
        }
    }
}
