using Protocol;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Color = UnityEngine.Color;

public enum eStone
{
    None = 0,
    BLACK = 1,
    WHITE = 2,
}

public class ClickHandler : MonoBehaviour
{
    public GameObject stonePrefab; // 바둑알 프리팹
    public GameObject stonePreviewPrefab; // 바둑알 미리보기 프리팹

    private GameObject currentStonePreview; // 바둑알 미리보기 프리팹
    private const float cell = 34.4f;
    private eStone[,] _stones;
    private eStone _stoneColor = eStone.None;
    private static ClickHandler _instance;
    public bool _myTurn = false;

    public static ClickHandler Instance
    {
        get
        {
            if (_instance == null)
            {
                // Scene에서 RoomManager를 찾아 인스턴스화
                _instance = FindObjectOfType<ClickHandler>();
            }
            return _instance;
        }
    }


    private void Start()
    {
        _stones = new eStone[19, 19];

        LobbySession.Instance.contentRecvEvent += OnRecvContent;
        BattleSession.Instance.contentRecvEvent += OnRecvContent;
    }

    private void OnRecvContent(P_GameContent pContent)
    {
        try
        {
            Vector2 pos = new Vector2 (pContent.XPos, pContent.YPos);
            PlaceStone(pos, (eStone)pContent.StoneColor);

             if((eStone)pContent.StoneColor != _stoneColor)
                _myTurn = true;           
        }
        catch (Exception e)
        {
            return;
        }
    }

    void Update()
    {
        if (_myTurn == false)
            return;

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
        GameObject newStone = Instantiate(stonePrefab, pPosition, Quaternion.identity);

        SpriteRenderer stoneRenderer = newStone.GetComponent<SpriteRenderer>();

        if (pColor == eStone.BLACK)
            stoneRenderer.color = Color.black;
        else if (pColor == eStone.WHITE)
            stoneRenderer.color = Color.white;
        

        // 인스턴스화된 돌의 위치를 업데이트하고, 게임 상태를 업데이트합니다.
        {
            int adjY = 9 + (int)(pPosition.y / 0.34);
            int adjX = 9 + (int)(pPosition.x / 0.34);

            _stones[adjY, adjX] = pColor;
        }
        _myTurn = false;
    }

    void PlaceStoneAndSend(Vector2 pPosition, eStone pColor)
    {
        PlaceStone(pPosition, pColor);

        Protocol.P_GameContent pkt = new Protocol.P_GameContent();
        pkt.RoomID = LobbyManager.Instance.RoomID;
        pkt.YPos = pPosition.y;
        pkt.XPos = pPosition.x;
        pkt.StoneColor = (int)pColor;
        byte[] sendBuffer = PacketHandler.SerializePacket(pkt, ePacketID.CONTENT_MESSAGE);

        BattleSession.Instance.Send(sendBuffer);
    }

    public void SetStoneColor(Color pColor)
    {
        stonePrefab.GetComponent<SpriteRenderer>().color = pColor;
        stonePreviewPrefab.GetComponent<SpriteRenderer>().color = pColor;

        if(pColor == Color.black)
            _stoneColor = eStone.BLACK;
        if (pColor == Color.white)
            _stoneColor = eStone.WHITE;
    }
}
