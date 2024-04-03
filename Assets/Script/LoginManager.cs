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
        // HTTP POST ��û�� ���� ��������Ʈ URL
        string url = "http://localhost:3000/login";

        string json = JsonConvert.SerializeObject(new { email = emailField.text, pwd = pwdField.text });
        // HttpClient �ν��Ͻ� ����
        using (HttpClient client = new HttpClient())
        {

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

                    user.user_name = jsonObj["name"].ToString();

                    Console.WriteLine("���� ����:");
                    //����
                    //return ret;
                    SceneChanger.ChangeLobbyScene();
                }
                else
                {
                    // ��û�� ������ ���
                    Console.WriteLine("��û ����: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                // ���� ó��
                Console.WriteLine("���� �߻�: " + e.Message);
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
