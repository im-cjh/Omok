using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUser : MonoBehaviour
{
    public Text _userName;
    GameObject childObject;

    public void Start()
    {
        childObject = gameObject.transform.Find("Name").gameObject;
        _userName = childObject.GetComponent<Text>();
    }
    public void SetInfo(string name)
    {
        _userName.text = name;
    }
}
