using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    //public string sceneName; // 전환할 씬의 이름을 Inspector에서 설정할 수 있도록 public 변수로 선언
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public static void ChangeLobbyScene()
    {
        SceneManager.LoadScene("Lobby 2"); // SceneManager를 사용하여 씬을 로드하고 전환
    }
}
