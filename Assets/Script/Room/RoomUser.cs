using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomUser : MonoBehaviour
{
    public Text _userName;

    public void SetInfo(string name)
    {
        _userName.text = name;
    }
}
