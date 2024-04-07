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
        SceneManager.LoadScene("Lobby"); // SceneManager�� ����Ͽ� ���� �ε��ϰ� ��ȯ
    }


    public static void ChangeGameScene()
    {
        SceneManager.LoadScene("Main");
    }
}
