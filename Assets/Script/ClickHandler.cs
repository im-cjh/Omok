using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

public class ClickHandler : MonoBehaviour
{
    public GameObject stonePrefab; // �ٵϾ� ������
    public GameObject stonePreviewPrefab; // �ٵϾ� �̸����� ������


    public Transform board; // �ٵ����� Transform

    private GameObject currentStonePreview; // �ٵϾ� �̸����� ������
    private const float cell = 34.4f;

    private Session _session;

    private void Start()
    {
        _session = FindObjectOfType<Session>();
    }

    void Update()
    {
        Vector2 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
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
            PlaceStone(pos);
        }
    }

    private void UpdatePreviewPosition(Vector2 position)
    {
        Vector2 clampedPos = new Vector2(Mathf.Clamp(position.x, -3.096f, 3.096f), Mathf.Clamp(position.y, -3.096f, 3.096f));
        //Vector3 clampedPos = position;
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

     void PlaceStone(Vector2 position)
    {
        //stonePrefab�� position�� ��ġ�ϸ� �˴ϴ�.
        Instantiate(stonePrefab, position, Quaternion.identity);

        //������ position����
        Task.Run(async () =>
        {
            await _session.Send<Vector2>(ePacketID.CONTENT_MESSAGE);
        });
    }
}
