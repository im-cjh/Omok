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
        // HTTP POST ��û�� ���� ��������Ʈ URL
        string url = "http://localhost:3000/login";

        string json = JsonConvert.SerializeObject(new { email = emailField.text, pwd = pwdField.text });
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
                    // ��û�� ������ ���
                    ID_text.text = "�̸��� - ��ȿ���� ���� ���̵� �Ǵ� ��й�ȣ�Դϴ�.";
                    ID_text.color = Color.red;
                    pwd_text.text = "��й�ȣ - ��ȿ���� ���� ���̵� �Ǵ� ��й�ȣ�Դϴ�.";
                    pwd_text.color = Color.red;
                }
            }
            catch (Exception e)
            {
                // ���� ó��
                messageManager.ShowMessage("��Ʈ��ũ�� Ȯ�����ּ���");
            }
        }
        //����
        //return null;
    }

    public void StartLogin()
    {
        LoginAsync();
    }
}
