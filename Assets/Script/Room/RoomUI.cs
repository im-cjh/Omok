using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

public struct Room
{
    //public string roomId;
    public string roomName;
    public string hostName;
    public Int32 numParticipants;
}

public class RoomUI : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI roomName;

    [SerializeField]
    public TextMeshProUGUI hostName;

    [SerializeField]
    TextMeshProUGUI numParticipants;

    void Init()
    {
        roomName = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        hostName = transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        numParticipants = transform.GetChild(2).GetComponent<TextMeshProUGUI>();
    }
    // Start is called before the first frame update
    public void SetRoomInfo(Room pRoom)
    {
        if(roomName == null) { Init();  }
        
        roomName.text = pRoom.roomName;
        hostName.text = pRoom.hostName;
        numParticipants.text = $"{pRoom.numParticipants}/2";   
    }
}
