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
    private Transform parentContent; //대화가 출력되는 ScrollView의 Content

    [SerializeField]
    private GameObject textChatPrefab; //대화를 출력하는 Text UI 프리팹

    private string _name; //임시 아이디

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
        //enter키를 누르면 대화 입력창에 입력된 내용을 대화창에 출력 
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            Protocol.C2SChatRoom pkt = new Protocol.C2SChatRoom();
            pkt.RoomID = LobbyManager.Instance.RoomID;
            pkt.SenderName = _name;
            pkt.Content = inputField.text;

            byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.CHAT_MESSAGE);
            BattleSession.Instance.Send(sendBuffer);
            //대화 입력창에 있는 내용 초기화 
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
        //대화 내용 출력을 위해 Text UI 생성 
        GameObject clone = Instantiate(textChatPrefab, parentContent);

        //대화 입력창에 있는 내용을 대화창에 출력(ID: 내용) 
        clone.GetComponent<TextMeshProUGUI>().text = $"{pChat.name}: {pChat.content}";
        clone.GetComponent<TextMeshProUGUI>().color = pChat.color;
        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.Return))
        {
            //대화 입력창이 포커스 되어있지 않을 때 Enter키를 누르면
            if (inputField.isFocused == false)
            {
                //대화 입력창의 포커스를 활성화 
                inputField.ActivateInputField();
            }
        }
    }
}
