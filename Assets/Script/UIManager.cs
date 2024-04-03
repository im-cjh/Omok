using UnityEngine;
using UnityEngine.UIElements;

public class UIManager : MonoBehaviour
{
    public UIDocument uiDocument; // UIManager�� ���� GameObject�� ����� UIDocument�� �Ҵ��մϴ�.
    public ListView listView; // ����Ʈ�並 ������ ����

    void Start()
    {
        // UIDocument�� ����Ͽ� UI�� �����ɴϴ�.
        VisualElement root = uiDocument.rootVisualElement;

        // ����Ʈ�並 �����ɴϴ�. (�ش� ����Ʈ���� �̸��� "listView"�� ���)
        listView = root.Q<ListView>("listView");

        // ����Ʈ�信 �����͸� �߰��մϴ�.
        AddDataToListView();
    }

    // ����Ʈ�信 �����͸� �߰��ϴ� �Լ�
    void AddDataToListView()
    {
        // ���÷� ����� ������ �迭�� �����մϴ�.
        string[] data = new string[] { "Item 1", "Item 2", "Item 3" };

        // �����͸� ����Ʈ�信 �߰��մϴ�.
        foreach (string itemData in data)
        {
            // ����Ʈ�� �������� �����մϴ�.
            VisualElement item = new VisualElement();

            // �����ۿ� �ؽ�Ʈ�� �߰��մϴ�.
            item.Add(new Label(itemData));

            // ����Ʈ�信 �������� �߰��մϴ�.
            listView.contentContainer.Add(item);
        }
    }
}
