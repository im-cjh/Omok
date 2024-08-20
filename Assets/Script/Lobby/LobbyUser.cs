using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUser : MonoBehaviour
{
    public Text _userName;
    public Text _userDesc;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            Canvas.ForceUpdateCanvases();
            _userName.text = User.Instance.userName;
            _userDesc.text = User.Instance.userWin + "½Â " + User.Instance.userLose + "ÆÐ";
        }
        catch (Exception ex)
        {
            Debug.Log(ex.Message);
            Debug.Log(User.Instance);
        }
    }
}