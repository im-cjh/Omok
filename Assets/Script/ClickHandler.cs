using UnityEngine;

public class ClickHandler : MonoBehaviour
{
    public GameObject stonePrefab; // �ٵϾ� ������
    public Transform board; // �ٵ����� Transform

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            if (pos.x < -3.3 || pos.x > 3.3 || pos.y < -3.3 || pos.y > 3.3)
                return;
            //    // �ٵ��� �������� Ŭ���� ��ġ
            //    Vector3 clickPosition = hit.point;
            //    // ��ǥ ����
            const float cx = 34.4f;
            const float cy = 34.4f;
            pos *= 100;
            float adjustedX = Mathf.Round(pos.x / cx) * cx;
            float adjustedY = Mathf.Round(pos.y / cy) * cy;
            
            pos.x = adjustedX/100; pos.y = adjustedY/100;
            //    // ���⼭ �ٵϾ��� ��ġ�ϴ� �ڵ� �ۼ�
            //    PlaceStone(boardPosition);
            //}
            PlaceStone(pos);
        }
    }

    void PlaceStone(Vector3 position)
    {
        position.z = -1;
        
        //stonePrefab�� position�� ��ġ�ϸ� �˴ϴ�.
        Instantiate(stonePrefab, position, Quaternion.identity);
    }
}
