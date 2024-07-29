using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    float sec = 0;  //Time.deltatime 이 float형태로 반환하기 때문에 float을 써준다.
    int Min = 0;

    public TextMeshProUGUI timetext;

    public void ResetTimer()
    {
        sec = 0;
        Min = 0;
        timetext.text = string.Format("{0:D2}:{1:D2}", Min, (int)sec);
    }


    private void Update()
    {
        sec += Time.deltaTime;
        timetext.text = string.Format("{0:D2}:{1:D2}", Min, (int)sec); //bool에서 float이엿기때문에 (int)넣어줌

        if ((int)sec > 59) //1분은 60초 이기 때문에 초는 59초 까지로 범위를 설정
        {
            sec = 0; //sec의 기본값은 0
            Min++;  //sec가 59보다 커질때 1분이 될때 Min(분) 은 커진다.
        }
    }

    public void OnCloseButtonPressed()
    {
        ResetTimer();
    }
}