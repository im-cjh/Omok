using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using PimDeWitte.UnityMainThreadDispatcher;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.Text.RegularExpressions;


static public class Utilities
{
    public static GameObject FindGameObject(string path)
    {
        GameObject obj = GameObject.Find(path);
        if (obj == null)
        {
            Debug.LogError($"{path} 오브젝트를 찾을 수 없습니다.");
        }
        return obj;
    }


    static public T FindAndAssign<T>(string path) where T : Component
    {
        GameObject obj = GameObject.Find(path);
        if (obj != null)
        {
            T component = obj.GetComponent<T>();
            if (component == null)
            {
                Debug.LogError($"{path} 오브젝트에서 {typeof(T).Name} 컴포넌트를 찾을 수 없습니다.");
            }
            return component;
        }
        else
        {
            Debug.LogError($"{path} 오브젝트를 찾을 수 없습니다.");
            return null;
        }
    }
    static public void WriteErrorLog(Exception e)
    {
        string path = Application.persistentDataPath + "/OmokLog.txt";
        try
        {
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(DateTime.Now.ToString());
                writer.WriteLine(e.Message);
                writer.WriteLine(e.StackTrace);
                writer.WriteLine();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError("Failed to write to log file: " + ex.Message);
        }
    }

    static private async Task UpdateWinRateAsync(int pId, bool pFlag)
    {
        Debug.Log("UpdateWinRateAsync");
        string url = "http://localhost:3000/update_win_rate";
        string json = JsonConvert.SerializeObject(new { userid = pId, flag = pFlag });

        try
        {
            using (HttpClient client = new HttpClient())
            {
                client.Timeout = TimeSpan.FromSeconds(3); // Timeout 설정

                HttpResponseMessage response = await client.PostAsync(url, new StringContent(json, Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    JObject jsonObj = JObject.Parse(jsonString);

                    UnityMainThreadDispatcher.Instance().Enqueue(async () =>
                    {
                    
                    });
                }
                else
                {
                    Debug.LogWarning("실패: " + response.StatusCode);
                }
            }
        }
        catch (HttpRequestException ex)
        {
            Debug.LogError("HTTP 요청 예외 발생: " + ex.Message);
        }
        catch (Exception ex)
        {
            Debug.Log("예외 발생: " + ex.Message);
        }
    }

    static public void StartUpdateWinRate(int pId, bool pFlag)
    {
        Task.Run(async () =>
        {
            await UpdateWinRateAsync(pId, pFlag);
        });
    }

    static public (int, int) ExtractWinLoss(string input)
    {
        // 정규 표현식을 사용하여 숫자 추출
        Regex regex = new Regex(@"\d+");
        MatchCollection matches = regex.Matches(input);

        if (matches.Count == 2)
        {
            int wins = int.Parse(matches[0].Value);
            int losses = int.Parse(matches[1].Value);
            return (wins, losses);
        }
        else
        {
            throw new ArgumentException("입력 문자열에서 숫자를 올바르게 추출할 수 없습니다.");
        }
    }
}

