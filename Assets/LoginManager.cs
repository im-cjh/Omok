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
        // HTTP POST ��û�� ���� ��������Ʈ URL
        string url = "http://localhost:3000/login";

        string json = JsonConvert.SerializeObject(new { email = emailField.text, pwd = pwdField.text });
        // HttpClient �ν��Ͻ� ����
        using (HttpClient client = new HttpClient())
        {
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var requestTask = client.PostAsync(url, content);
            yield return new WaitUntil(() => requestTask.IsCompleted);

            try
            {
                // HTTP POST ��û�� ����� �����մϴ�.

                // ��û�� �Ϸ�� ������ ����մϴ�.

                // ���� �޽����� Ȯ���մϴ�.
                var response = requestTask.Result;
                if (response.IsSuccessStatusCode)
                {
                    // ���������� ��û�� �Ϸ�Ǿ��� ��
                    var responseBody =  response.Content.ReadAsStringAsync();
                    Debug.Log("���� ����:");
                    Debug.Log(responseBody);
                }
                else
                {
                    // ��û�� ������ ���
                    Debug.Log("��û ����: " + response.StatusCode);
                }
            }
            catch (Exception e)
            {
                // ���� ó��
                Debug.Log("���� �߻�: " + e.Message);
            }
        }
    }

    public void StartLoginCoroutine()
    {
        StartCoroutine(LoginCoroutine());
    }
}
