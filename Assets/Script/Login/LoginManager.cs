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
    public Text SignInID_text;
    public Text SignInPwd_text;
    public TMP_InputField SignInEmailField;
    public TMP_InputField SignInPwdField;

    public TMP_InputField SignUpEmailField;
    public TMP_InputField SignUpPwdField;
    public TMP_InputField SignUpNameField;

    private void Start()
    {
        // ���� �޼��带 ����Ͽ� �� TMP_InputField�� �Ҵ��մϴ�.
        SignUpEmailField = Utilities.FindAndAssign<TMP_InputField>("Canvas/SignUpPanel/SignUpBox/email_input");
        SignUpPwdField = Utilities.FindAndAssign<TMP_InputField>("Canvas/SignUpPanel/SignUpBox/pwd_input");
        SignUpNameField = Utilities.FindAndAssign<TMP_InputField>("Canvas/SignUpPanel/SignUpBox/name_input");
    }

    private async Task SignInAsync()
    {
        // HTTP POST ��û�� ���� ��������Ʈ URL
        string url = "http://localhost:3000/login";

        string json = JsonConvert.SerializeObject(new { email = SignInEmailField.text, pwd = SignInPwdField.text });
        // HttpClient �ν��Ͻ� ����
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
                    var rc = await response.Content.ReadAsStringAsync();
                    // ���������� ��û�� �Ϸ�Ǿ��� ��
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    User.Instance.userName = jsonObj["name"].ToString();
                    User.Instance.id = Convert.ToInt32(jsonObj["id"].ToString());
                    
                    //����
                    //return ret;
                    LobbySession.Instance.Connect("127.0.0.1", 7777);

                    Protocol.C2SLoginSuccess pkt = new Protocol.C2SLoginSuccess();
                    pkt.UserName = User.Instance.userName;
                    pkt.UserID = User.Instance.id;

                    byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.LOGIN_SUCCESS);
                    await LobbySession.Instance.Send(sendBuffer);

                    SceneChanger.ChangeLobbyScene();
                }
                else
                {
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
        Debug.Log("sibal");
        Debug.Log(SignUpEmailField.text);
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
                        MessageManager.Instance.ShowMessage("ȸ�������� ���������� �Ϸ�Ǿ����ϴ�.");
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
