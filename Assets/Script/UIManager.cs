using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public UIDocument uiDocument; // UIManager가 속한 GameObject에 연결된 UIDocument를 할당합니다.
    public ListView listView; // 리스트뷰를 저장할 변수

    void Start()
    {
        // UIDocument를 사용하여 UI를 가져옵니다.
        VisualElement root = uiDocument.rootVisualElement;

        // 리스트뷰를 가져옵니다. (해당 리스트뷰의 이름이 "listView"일 경우)
        listView = root.Q<ListView>("listView");

        // 리스트뷰에 데이터를 추가합니다.
        AddDataToListView();
    }

    // 리스트뷰에 데이터를 추가하는 함수
    void AddDataToListView()
    {
        // 예시로 사용할 데이터 배열을 생성합니다.
        string[] data = new string[] { "Item 1", "Item 2", "Item 3" };

        // 데이터를 리스트뷰에 추가합니다.
        foreach (string itemData in data)
        {
            // 리스트뷰 아이템을 생성합니다.
            VisualElement item = new VisualElement();

            // 아이템에 텍스트를 추가합니다.
            item.Add(new Label(itemData));

            // 리스트뷰에 아이템을 추가합니다.
            listView.contentContainer.Add(item);
        }
    }
}
