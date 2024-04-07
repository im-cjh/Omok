using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ClickHandler : MonoBehaviour
{
    public GameObject stonePrefab; // �ٵϾ� ������
    public GameObject stonePreviewPrefab; // �ٵϾ� �̸����� ������


    public Transform board; // �ٵ����� Transform

    private GameObject currentStonePreview; // �ٵϾ� �̸����� ������
    private const float cell = 34.4f;

    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //��ǥ ����
        float adjustedX = (Mathf.Round((pos.x*100) / cell) * cell)/100;
        float adjustedY = (Mathf.Round((pos.y*100) / cell) * cell)/100;

        //���� ���� ���� �̸�����
        UpdatePreviewPosition(new Vector3(adjustedX, adjustedY, 0f));

        if (Input.GetMouseButtonDown(0))
        {

            if (pos.x < -3.3 || pos.x > 3.3 || pos.y < -3.3 || pos.y > 3.3)
                return;
            
            pos.x = adjustedX; 
            pos.y = adjustedY;
            pos.z = -1;
            PlaceStone(pos);
        }
    }

    private void UpdatePreviewPosition(Vector3 position)
    {
        Vector3 clampedPos = new Vector3(Mathf.Clamp(position.x, -3.096f, 3.096f), Mathf.Clamp(position.y, -3.096f, 3.096f), position.z);
        // �̸����� ������Ʈ�� �����ϰų� ��ġ ������Ʈ
        if (currentStonePreview == null)
        {
            currentStonePreview = Instantiate(stonePreviewPrefab, clampedPos, Quaternion.identity);
        }
        else
        {
            currentStonePreview.transform.position = clampedPos;
        }
    }

    void PlaceStone(Vector3 position)
    {
        //stonePrefab�� position�� ��ġ�ϸ� �˴ϴ�.
        Instantiate(stonePrefab, position, Quaternion.identity);

        //������ position����
    }
}
