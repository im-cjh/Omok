using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyUser : MonoBehaviour
{
    public Text _userName;

    // Start is called before the first frame update
    void Start()
    {
        string un = FindObjectOfType<User>().userName;
        _userName.text = un;
    }
}
