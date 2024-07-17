using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManagerChecker : MonoBehaviour
{
    private TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();

    void Start()
    {
        DontDestroyOnLoad(this);
        StartCoroutine(WaitForGameManager());
    }

    private IEnumerator WaitForGameManager()
    {
        while (SceneManager.GetActiveScene().name != "Main")
        {
            yield return null;
        }

        // 조건이 만족되면 TaskCompletionSource 완료
        Debug.Log("조건이 만족되면 TaskCompletionSource 완료");
        tcs.SetResult(true);
    }

    public async Task WaitForGameManagerAsync()
    {
        await tcs.Task;
        Debug.Log("GameManager is now available.");
    }
}
