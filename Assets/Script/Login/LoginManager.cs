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

    public MessageManager messageManager;

    private void Start()
    {
        SignInID_text = Utilities.FindAndAssign<Text>("Canvas/SignInPanel/SignInBox/email_text");
        SignInPwd_text = Utilities.FindAndAssign<Text>("Canvas/SignInPanel/SignInBox/pwd_text");
        SignInEmailField = Utilities.FindAndAssign<TMP_InputField>("Canvas/SignInPanel/SignInBox/email_input");
        SignInPwdField = Utilities.FindAndAssign<TMP_InputField>("Canvas/SignInPanel/SignInBox/pwd_input");
    }

    private async Task SignInAsync()
    {
        string url = "http://localhost:3000/login";
        string json = JsonConvert.SerializeObject(new { email = SignInEmailField.text, pwd = SignInPwdField.text });

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(3); // Timeout ����

                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    UnityMainThreadDispatcher.Instance().Enqueue(async () =>
                    {
                        User.Instance.userName = jsonObj["name"].ToString();
                        User.Instance.id = Convert.ToInt32(jsonObj["id"].ToString());
                        Debug.Log("�α��� ����!");
                        Debug.Log(User.Instance.userName);

                        // �߰� �۾� ����
                        LobbySession.Instance.Connect("127.0.0.1", 7777);

                        Protocol.C2SLoginSuccess pkt = new Protocol.C2SLoginSuccess();
                        pkt.UserName = jsonObj["name"].ToString();
                        pkt.UserID = Convert.ToInt32(jsonObj["id"].ToString());

                        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.LOGIN_SUCCESS);
                        await LobbySession.Instance.Send(sendBuffer);

                        SceneChanger.ChangeLobbyScene();
                    });
                }
                else
                {
                    Debug.LogWarning("�α��� ����: " + response.StatusCode);
                    SignInID_text.text = "�̸��� - ��ȿ���� ���� ���̵� �Ǵ� ��й�ȣ�Դϴ�.";
                    SignInID_text.color = Color.red;
                    SignInPwd_text.text = "��й�ȣ - ��ȿ���� ���� ���̵� �Ǵ� ��й�ȣ�Դϴ�.";
                    SignInPwd_text.color = Color.red;
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Debug.LogError("HTTP ��û ���� �߻�: " + ex.Message);
            messageManager.ShowMessage("��Ʈ��ũ ������ Ȯ�����ּ���.");
        }
        catch (Exception ex)
        {
            Debug.Log("���� �߻�: " + ex.Message);
            messageManager.ShowMessage("��Ʈ��ũ ���ῡ �����߽��ϴ�.");
        }
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
                        messageManager.ShowMessage("ȸ�������� �Ϸ�Ǿ����ϴ�. �α��� ���ּ���");
                        messageManager.CloseSignUpPanel();
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
                    messageManager.ShowMessage("��Ʈ��ũ�� Ȯ�����ּ���");
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
