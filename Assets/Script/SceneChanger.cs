using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    //public string sceneName; // ��ȯ�� ���� �̸��� Inspector���� ������ �� �ֵ��� public ������ ����
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public static void ChangeLobbyScene()
    {
        SceneManager.LoadScene("Lobby 2"); // SceneManager�� ����Ͽ� ���� �ε��ϰ� ��ȯ
    }
}
