using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Timer : MonoBehaviour
{
    float sec = 0;  //Time.deltatime �� float���·� ��ȯ�ϱ� ������ float�� ���ش�.
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
        timetext.text = string.Format("{0:D2}:{1:D2}", Min, (int)sec); //bool���� float�̿��⶧���� (int)�־���

        if ((int)sec > 59) //1���� 60�� �̱� ������ �ʴ� 59�� ������ ������ ����
        {
            sec = 0; //sec�� �⺻���� 0
            Min++;  //sec�� 59���� Ŀ���� 1���� �ɶ� Min(��) �� Ŀ����.
        }
    }

    public void OnCloseButtonPressed()
    {
        ResetTimer();
    }
}