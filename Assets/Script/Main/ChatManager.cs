using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class  ChatStruct
{
    public string name;
    public string content;
    public Color color = Color.white;
}

public class ChatManager : MonoBehaviour
{
    private static ChatManager _instance;

    public ScrollRect scrollView;

    [SerializeField]
    private TMP_InputField inputField;

    [SerializeField]
    private Transform parentContent; //��ȭ�� ��µǴ� ScrollView�� Content

    [SerializeField]
    private GameObject textChatPrefab; //��ȭ�� ����ϴ� Text UI ������

    private string _name; //�ӽ� ���̵�

    public static ChatManager Instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<ChatManager>();
            
            return _instance;
        }
    }

    private void Start()
    {
        _name = User.Instance.userName;
        LobbySession.Instance.chatRoomRecvEvent += UpdateChat;
        BattleSession.Instance.chatRoomRecvEvent += UpdateChat;
    }
    public void OnEndEditEventMethod()
    {
        //enterŰ�� ������ ��ȭ �Է�â�� �Էµ� ������ ��ȭâ�� ��� 
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Protocol.C2SChatRoom pkt = new Protocol.C2SChatRoom();
            pkt.RoomID = LobbyManager.Instance.RoomID;
            pkt.SenderName = _name;
            pkt.Content = inputField.text;

            byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.CHAT_MESSAGE);
            BattleSession.Instance.Send(sendBuffer);
            //��ȭ �Է�â�� �ִ� ���� �ʱ�ȭ 
            inputField.text = "";

        }

    }

    public void UpdateChat(ChatStruct pChat)
    {
        if (pChat.content.Equals(""))
        {
            Debug.Log("UpdateChat2");
            return;
        }
        //��ȭ ���� ����� ���� Text UI ���� 
        GameObject clone = Instantiate(textChatPrefab, parentContent);

        //��ȭ �Է�â�� �ִ� ������ ��ȭâ�� ���(ID: ����) 
        clone.GetComponent<TextMeshProUGUI>().text = $"{pChat.name}: {pChat.content}";
        clone.GetComponent<TextMeshProUGUI>().color = pChat.color;
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
        }
    }
}
