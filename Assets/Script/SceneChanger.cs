using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    public string sceneName; // ��ȯ�� ���� �̸��� Inspector���� ������ �� �ֵ��� public ������ ����

    public void ChangeScene()
    {
        SceneManager.LoadScene(sceneName); // SceneManager�� ����Ͽ� ���� �ε��ϰ� ��ȯ
    }
}
