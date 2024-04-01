using MongoDB.Bson.IO;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LoginManager : MonoBehaviour
{
    [SerializeField]
    private TMP_InputField emailField;

    [SerializeField]
    private TMP_InputField pwdField;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator LoginCoroutine()
    {
        // HTTP POST 요청을 보낼 엔드포인트 URL
        string url = "http://localhost:3000/login";

        string json = JsonConvert.SerializeObject(new { email = emailField.text, pwd = pwdField.text });
        // HttpClient 인스턴스 생성
        using (HttpClient client = new HttpClient())
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var requestTask = client.PostAsync(url, content);
            yield return new WaitUntil(() => requestTask.IsCompleted);

            try
            {
                // HTTP POST 요청을 만들고 전송합니다.

                // 요청이 완료될 때까지 대기합니다.

                // 응답 메시지를 확인합니다.
                var response = requestTask.Result;
                if (response.IsSuccessStatusCode)
                {
                    // 성공적으로 요청이 완료되었을 때
                    var responseBody =  response.Content.ReadAsStringAsync();
                    Debug.Log("서버 응답:");
                    Debug.Log(responseBody);
                }
                else
                {
                    // 요청이 실패한 경우
                    Debug.Log("요청 실패: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                // 오류 처리
                Debug.Log("오류 발생: " + e.Message);
            }
        }
    }

    public void StartLoginCoroutine()
    {
        StartCoroutine(LoginCoroutine());
    }
}
