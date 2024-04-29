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
    private Transform parentContent; //대화가 출력되는 ScrollView의 Content

    [SerializeField]
    private GameObject textChatPrefab; //대화를 출력하는 Text UI 프리팹

    private string ID = "Docker"; //임시 아이디

    public void OnEndEditEventMethod()
    {
        //enter키를 누르면 대화 입력창에 입력된 내용을 대화창에 출력 
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

        //대화 내용 출력을 위해 Text UI 생성 
        GameObject clone = Instantiate(textChatPrefab, parentContent);

        //대화 입력창에 있는 내용을 대화창에 출력(ID: 내용) 
        clone.GetComponent<TextMeshProUGUI>().text = $"{ID}: {inputField.text}";

        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;

        //대화 입력창에 있는 내용 초기화 
        inputField.text = "";
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
            else
            {
                //대화 내용 전송

            }
        }
    }
}
