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
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public MessageManager messageManager;


    [SerializeField]
    private Text ID_text;

    [SerializeField]
    private Text pwd_text;

    [SerializeField]
    private TMP_InputField emailField;

    [SerializeField]
    private TMP_InputField pwdField;

    private Session _session;

    private void Start()
    {
        _session = FindObjectOfType<Session>();
    }

    private async Task LoginAsync()
    {
        // HTTP POST 요청을 보낼 엔드포인트 URL
        string url = "http://localhost:3000/login";

        string json = JsonConvert.SerializeObject(new { email = emailField.text, pwd = pwdField.text });
        // HttpClient 인스턴스 생성
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(5); // 10초로 시간 제한 설정
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

                    User.Instance.userName = jsonObj["name"].ToString();
                    User.Instance.id = Convert.ToInt32(jsonObj["id"].ToString());
                    
                    //성공
                    //return ret;
                    await _session.Connect();

                    Protocol.C2SLoginSuccess pkt = new Protocol.C2SLoginSuccess();
                    pkt.UserName = User.Instance.userName;
                    pkt.UserID = User.Instance.id;

                    byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.LOGIN_SUCCESS);
                    await _session.Send(sendBuffer);

                    SceneChanger.ChangeLobbyScene();
                }
                else
                {
                    // 요청이 실패한 경우
                    ID_text.text = "이메일 - 유효하지 않은 아이디 또는 비밀번호입니다.";
                    ID_text.color = Color.red;
                    pwd_text.text = "비밀번호 - 유효하지 않은 아이디 또는 비밀번호입니다.";
                    pwd_text.color = Color.red;
                }
            }
            catch (Exception e)
            {
                // 오류 처리
                messageManager.ShowMessage("네트워크를 확인해주세요");
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
