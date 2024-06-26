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
 
        // HTTP POST ��û�� ���� ��������Ʈ URL
        string url = "http://localhost:3000/login";
        Debug.Log(url);
        string json = JsonConvert.SerializeObject(new { email = SignInEmailField.text, pwd = SignInPwdField.text });
        Debug.Log(json);

        //HttpClient �ν��Ͻ� ����
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(5); // 10�ʷ� �ð� ���� ����
            try
            {
                // HTTP POST ��û�� ����� �����մϴ�.
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));
                
                // ���� �޽����� Ȯ���մϴ�.
                if (response.IsSuccessStatusCode)
                {
                    Debug.Log(response.IsSuccessStatusCode);
                    var rc = await response.Content.ReadAsStringAsync();
                    // ���������� ��û�� �Ϸ�Ǿ��� ��
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        User.Instance.userName = jsonObj["name"].ToString();
                        User.Instance.id = Convert.ToInt32(jsonObj["id"].ToString());
                        Debug.Log("����");
                        LobbySession.Instance.Connect("127.0.0.1", 7777);
                    


                        Protocol.C2SLoginSuccess pkt = new Protocol.C2SLoginSuccess();
                        pkt.UserName = jsonObj["name"].ToString();
                        pkt.UserID = Convert.ToInt32(jsonObj["id"].ToString());

                        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.LOGIN_SUCCESS);
                        LobbySession.Instance.Send(sendBuffer);
                        //����


                        SceneChanger.ChangeLobbyScene();
                    });

            }
                else
                {
                    Debug.Log("����");
                    // ��û�� ������ ���
                    SignInID_text.text = "�̸��� - ��ȿ���� ���� ���̵� �Ǵ� ��й�ȣ�Դϴ�.";
                    SignInID_text.color = Color.red;
                    SignInPwd_text.text = "��й�ȣ - ��ȿ���� ���� ���̵� �Ǵ� ��й�ȣ�Դϴ�.";
                    SignInPwd_text.color = Color.red;
                }
            }
            catch (Exception e)
            {
                // ���� ó��
                MessageManager.Instance.ShowMessage("��Ʈ��ũ�� Ȯ�����ּ���");
                MessageManager.Instance.CloseSignUpPanel();
            }
        }
        //����
        //return null;
    }

    public async Task SignUpAsync()
    {
        
        // HTTP POST ��û�� ���� ��������Ʈ URL
        string url = "http://localhost:3000/signup";

        
        // ����� �Է� �����͸� JSON �������� ����ȭ
        string json = JsonConvert.SerializeObject(new
        {
            email = SignUpEmailField.text,
            pwd = SignUpPwdField.text,
            name = SignUpNameField.text
        });
        Debug.Log(json);

        // HttpClient �ν��Ͻ� ����
        using (HttpClient client = new HttpClient())
        {
            client.Timeout = TimeSpan.FromSeconds(5); // �ð� ���� ����
            try
            {
                // HTTP POST ��û�� ����� ����
                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                // ���� �޽����� Ȯ��
                if (response.IsSuccessStatusCode)
                {
                    var rc = await response.Content.ReadAsStringAsync();
                    // ���������� ��û�� �Ϸ�Ǿ��� ��
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    // ȸ������ ���� �޽����� ǥ��
                    UnityMainThreadDispatcher.Instance().Enqueue(() =>
                    {
                        MessageManager.Instance.ShowMessage("ȸ�������� �Ϸ�Ǿ����ϴ�. �α��� ���ּ���");
                        MessageManager.Instance.CloseSignUpPanel();
                    });
                }
                else
                {
                    // ��û�� ������ ���
                    SignInID_text.text = "�̸��� - �̹� �����ϴ� �̸����Դϴ�.";
                    SignInID_text.color = Color.red;
                    SignInPwd_text.text = "��й�ȣ - ��ȿ���� ���� ��й�ȣ�Դϴ�.";
                    SignInPwd_text.color = Color.red;
                }
            }
            catch (Exception e)
            {
                // ���� ó��
                Debug.Log("Ȯ�����ּ���");
                Debug.Log(e.Message);

                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    MessageManager.Instance.ShowMessage("��Ʈ��ũ�� Ȯ�����ּ���");
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
