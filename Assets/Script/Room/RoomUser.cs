using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUser : MonoBehaviour
{
    public Text _userName;
    public Text _userWinRate;
    public SpriteRenderer stoneType;
    public eStone _stoneColor;

    GameObject childObject;

    public void Start()
    {
        _userName = gameObject.transform.Find("Name").gameObject.GetComponent<Text>(); ;
        _userWinRate = gameObject.transform.Find("Desc").gameObject.GetComponent<Text>(); ;
        stoneType = gameObject.transform.Find("StoneType").gameObject.GetComponent<SpriteRenderer>();

        _userWinRate.text = User.Instance.userWin + "½Â " + User.Instance.userLose + "ÆÐ";
    }
    public void SetInfo(string name)
    {
        _userName.text = name;
    }

    public void SetStoneColor(Color pColor)
    {
        stoneType.color = pColor;
        if (pColor == Color.black)
            _stoneColor = eStone.BLACK;
        else if (pColor == Color.white)
            _stoneColor = eStone.WHITE;
        else
            Debug.LogError("SetStoneColor: Unexpected Color " + pColor);
    }

    public void SetWinRate(Int32 pWin, Int32 pLose)
    {
        _userWinRate.text = pWin + "½Â " + pLose + "ÆÐ";
    }
}
