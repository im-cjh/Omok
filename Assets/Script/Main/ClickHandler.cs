using Protocol;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

enum eStone
{
    None = 0,
    BLACK = 1,
    WHITE = 2,
}

public class ClickHandler : MonoBehaviour
{
    public GameObject stonePrefab; // 바둑알 프리팹
    public GameObject stonePreviewPrefab; // 바둑알 미리보기 프리팹


    public Transform board; // 바둑판의 Transform

    private GameObject currentStonePreview; // 바둑알 미리보기 프리팹
    private const float cell = 34.4f;

    private Session _session;
    private eStone[,] _stones;
    private eStone _stoneColor = eStone.BLACK;

    private void Start()
    {
        _stones = new eStone[19, 19];
        _session = FindObjectOfType<Session>();
        _session.contentRecvEvent += OnRecvContent;
    }

    private void OnRecvContent(P_GameContent pContent)
    {
        try
        {
            Vector2 pos = new Vector2 (pContent.XPos, pContent.YPos);
            PlaceStone(pos, (eStone)pContent.StoneColor);
        }
        catch (Exception e)
        {
            return;
        }
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
            PlaceStoneAndSend(pos, _stoneColor);
        }
    }

    private void UpdatePreviewPosition(Vector2 pPosition)
    {
        Vector2 clampedPos = new Vector2(Mathf.Clamp(pPosition.x, -3.096f, 3.096f), Mathf.Clamp(pPosition.y, -3.096f, 3.096f));
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

    void PlaceStone(Vector2 pPosition, eStone pColor)
    {
        Debug.Log("sadsa");
        //stonePrefab을 position에 배치하면 됩니다.
        Instantiate(stonePrefab, pPosition, Quaternion.identity);

        {
            int adjY = 9+(int)(pPosition.y / 0.34);
            int adjX = 9 + (int)(pPosition.x / 0.34);
            Debug.Log(adjY + ", " + adjX + ")");
            _stones[adjY, adjX] = _stoneColor;
        }
    }

    void PlaceStoneAndSend(Vector2 pPosition, eStone pColor)
    {
        PlaceStone(pPosition, pColor);

        Protocol.P_GameContent pkt = new Protocol.P_GameContent();
        pkt.RoomID = LobbyManager.Instance.GetSelectedRoom().roomId;
        Debug.Log("RoomID: " + pkt.RoomID);
        pkt.YPos = pPosition.y;
        pkt.XPos = pPosition.x;
        pkt.StoneColor = (int)pColor;
        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.CONTENT_MESSAGE);

        _session.Send(sendBuffer);

        //서버에 position전송
        //Task.Run(async () =>
        //{
        //    Protocol.P_GameContent pkt = new Protocol.P_GameContent();
        //    pkt.RoomID = LobbyManager.Instance.GetSelectedRoomID();
        //    Debug.Log("RoomID: " + pkt.RoomID);
        //    pkt.XPos = pPosition.x;
        //    pkt.YPos = pPosition.y;
        //    pkt.StoneColor = (int)pColor;
        //    byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.CONTENT_MESSAGE);

        //    await _session.Send(sendBuffer);
        //});

    }
}
