using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

    public User user;

    private async Task LoginAsync()
    {
        // HTTP POST 요청을 보낼 엔드포인트 URL
        string url = "http://localhost:3000/login";

        string json = JsonConvert.SerializeObject(new { email = emailField.text, pwd = pwdField.text });
        // HttpClient 인스턴스 생성
        using (HttpClient client = new HttpClient())
        {

            try
            {
                // HTTP POST 요청을 만들고 전송합니다.
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                // 응답 메시지를 확인합니다.
                if (response.IsSuccessStatusCode)
                {
                    var rc = await response.Content.ReadAsStringAsync();
                    // 성공적으로 요청이 완료되었을 때
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    user.user_name = jsonObj["name"].ToString();

                    Console.WriteLine("서버 응답:");
                    //성공
                    //return ret;
                    SceneChanger.ChangeLobbyScene();
                }
                else
                {
                    // 요청이 실패한 경우
                    Console.WriteLine("요청 실패: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                // 오류 처리
                Console.WriteLine("오류 발생: " + e.Message);
            }
        }
        //실패
        //return null;
    }

    public void StartLogin()
    {
        LoginAsync();
    }
}
