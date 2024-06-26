using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class LoginManager : MonoBehaviour
{  
    private Text SignInID_text;
    private Text SignInPwd_text;
    private TMP_InputField SignInEmailField;
    private TMP_InputField SignInPwdField;

    public TMP_InputField SignUpEmailField;
    public TMP_InputField SignUpPwdField;
    public TMP_InputField SignUpNameField;

    private void Start()
    {
        SignInID_text = Utilities.FindAndAssign<Text>("Canvas/SignInPanel/SignInBox/email_text");
        SignInPwd_text = Utilities.FindAndAssign<Text>("Canvas/SignInPanel/SignInBox/pwd_text");
        SignInEmailField = Utilities.FindAndAssign<TMP_InputField>("Canvas/SignInPanel/SignInBox/email_input");
        SignInPwdField = Utilities.FindAndAssign<TMP_InputField>("Canvas/SignInPanel/SignInBox/pwd_input");
    }

    private async Task SignInAsync()
    {
 
        // HTTP POST 요청을 보낼 엔드포인트 URL
        string url = "http://localhost:3000/login";
        Debug.Log(url);
        string json = JsonConvert.SerializeObject(new { email = SignInEmailField.text, pwd = SignInPwdField.text });
        Debug.Log(json);

        //HttpClient 인스턴스 생성
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
                    Debug.Log(response.IsSuccessStatusCode);
                    var rc = await response.Content.ReadAsStringAsync();
                    // 성공적으로 요청이 완료되었을 때
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        User.Instance.userName = jsonObj["name"].ToString();
                        User.Instance.id = Convert.ToInt32(jsonObj["id"].ToString());
                        Debug.Log("성공");
                        LobbySession.Instance.Connect("127.0.0.1", 7777);
                    


                        Protocol.C2SLoginSuccess pkt = new Protocol.C2SLoginSuccess();
                        pkt.UserName = jsonObj["name"].ToString();
                        pkt.UserID = Convert.ToInt32(jsonObj["id"].ToString());

                        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.LOGIN_SUCCESS);
                        LobbySession.Instance.Send(sendBuffer);
                        //성공


                        SceneChanger.ChangeLobbyScene();
                    });

            }
                else
                {
                    Debug.Log("실패");
                    // 요청이 실패한 경우
                    SignInID_text.text = "이메일 - 유효하지 않은 아이디 또는 비밀번호입니다.";
                    SignInID_text.color = Color.red;
                    SignInPwd_text.text = "비밀번호 - 유효하지 않은 아이디 또는 비밀번호입니다.";
                    SignInPwd_text.color = Color.red;
                }
            }
            catch (Exception e)
            {
                // 오류 처리
                MessageManager.Instance.ShowMessage("네트워크를 확인해주세요");
                MessageManager.Instance.CloseSignUpPanel();
            }
        }
        //실패
        //return null;
    }

    public async Task SignUpAsync()
    {
        
        // HTTP POST 요청을 보낼 엔드포인트 URL
        string url = "http://localhost:3000/signup";

        
        // 사용자 입력 데이터를 JSON 형식으로 직렬화
        string json = JsonConvert.SerializeObject(new
        {
            email = SignUpEmailField.text,
            pwd = SignUpPwdField.text,
            name = SignUpNameField.text
        });
        Debug.Log(json);

        // HttpClient 인스턴스 생성
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(5); // 시간 제한 설정
            try
            {
                // HTTP POST 요청을 만들고 전송
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                // 응답 메시지를 확인
                if (response.IsSuccessStatusCode)
                {
                    var rc = await response.Content.ReadAsStringAsync();
                    // 성공적으로 요청이 완료되었을 때
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    // 회원가입 성공 메시지를 표시
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        MessageManager.Instance.ShowMessage("회원가입이 완료되었습니다. 로그인 해주세요");
                        MessageManager.Instance.CloseSignUpPanel();
                    });
                }
                else
                {
                    // 요청이 실패한 경우
                    SignInID_text.text = "이메일 - 이미 존재하는 이메일입니다.";
                    SignInID_text.color = Color.red;
                    SignInPwd_text.text = "비밀번호 - 유효하지 않은 비밀번호입니다.";
                    SignInPwd_text.color = Color.red;
                }
            }
            catch (Exception e)
            {
                // 오류 처리
                Debug.Log("확인해주세요");
                Debug.Log(e.Message);

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    MessageManager.Instance.ShowMessage("네트워크를 확인해주세요");
                });
            }
        }
    }

    public void StartSignIn()
    {
        Task.Run(async () =>
        {
            await SignInAsync();
        });
    }

    public void StartSignUp()
    {

        Task.Run(async () =>
        {
            await SignUpAsync();
        });
    }
}
