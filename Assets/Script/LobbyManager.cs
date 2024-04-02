using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    public ScrollRect scrollView;

    [SerializeField]
    private Transform parentContent; //대화가 출력되는 ScrollView의 Content

    [SerializeField]
    private GameObject textChatPrefab; //대화를 출력하는 Text UI 프리팹

    public void OnEndEditEventMethod()
    {
            UpdateChat();
    }

    public void UpdateChat()
    {
        //대화 내용 출력을 위해 Text UI 생성 
        GameObject clone = Instantiate(textChatPrefab, parentContent);

        //대화 입력창에 있는 내용을 대화창에 출력(ID: 내용) 
        //clone.GetComponent<TextMeshProUGUI>().text = $"{ID}: {inputField.text}";

        Canvas.ForceUpdateCanvases();
        scrollView.verticalNormalizedPosition = 0f;
    }
}
