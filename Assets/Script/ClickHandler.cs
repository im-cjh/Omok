using System;
using UnityEngine;
using UnityEngine.UIElements;

public class ClickHandler : MonoBehaviour
{
    public GameObject stonePrefab; // 바둑알 프리팹
    public GameObject stonePreviewPrefab; // 바둑알 미리보기 프리팹


    public Transform board; // 바둑판의 Transform

    private GameObject currentStonePreview; // 바둑알 미리보기 프리팹
    private const float cell = 34.4f;

    void Update()
    {
        Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //좌표 보정
        float adjustedX = (Mathf.Round((pos.x*100) / cell) * cell)/100;
        float adjustedY = (Mathf.Round((pos.y*100) / cell) * cell)/100;

        //예상 착수 지점 미리보기
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
        // 미리보기 오브젝트를 생성하거나 위치 업데이트
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
        //stonePrefab을 position에 배치하면 됩니다.
        Instantiate(stonePrefab, position, Quaternion.identity);

        //서버에 position전송
    }
}
