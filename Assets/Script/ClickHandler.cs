using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class ClickHandler : MonoBehaviour
{
    public GameObject stonePrefab; // 바둑알 프리팹
    public GameObject stonePreviewPrefab; // 바둑알 미리보기 프리팹


    public Transform board; // 바둑판의 Transform

    private GameObject currentStonePreview; // 바둑알 미리보기 프리팹
    private const float cell = 34.4f;

    private Session _session;

    private void Start()
    {
        _session = FindObjectOfType<Session>();
    }

    void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
            PlaceStone(pos);
        }
    }

    private void UpdatePreviewPosition(Vector2 position)
    {
        Vector2 clampedPos = new Vector2(Mathf.Clamp(position.x, -3.096f, 3.096f), Mathf.Clamp(position.y, -3.096f, 3.096f));
        //Vector3 clampedPos = position;
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

     void PlaceStone(Vector2 position)
    {
        //stonePrefab을 position에 배치하면 됩니다.
        Instantiate(stonePrefab, position, Quaternion.identity);

        //서버에 position전송
        Task.Run(async () =>
        {
            await _session.Send<Vector2>(ePacketID.CONTENT_MESSAGE);
        });
    }
}
