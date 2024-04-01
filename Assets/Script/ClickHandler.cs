using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public GameObject stonePrefab; // 바둑알 프리팹
    public Transform board; // 바둑판의 Transform

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (pos.x < -3.3 || pos.x > 3.3 || pos.y < -3.3 || pos.y > 3.3)
                return;
            //    // 바둑판 위에서의 클릭된 위치
            //    Vector3 clickPosition = hit.point;
            //    // 좌표 보정
            const float cx = 34.4f;
            const float cy = 34.4f;
            pos *= 100;
            float adjustedX = Mathf.Round(pos.x / cx) * cx;
            float adjustedY = Mathf.Round(pos.y / cy) * cy;
            
            pos.x = adjustedX/100; pos.y = adjustedY/100;
            //    // 여기서 바둑알을 배치하는 코드 작성
            //    PlaceStone(boardPosition);
            //}
            PlaceStone(pos);
        }
    }

    void PlaceStone(Vector3 position)
    {
        position.z = -1;
        
        //stonePrefab을 position에 배치하면 됩니다.
        Instantiate(stonePrefab, position, Quaternion.identity);
    }
}
