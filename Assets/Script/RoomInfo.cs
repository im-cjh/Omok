using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RoomInfo : MonoBehaviour
{
    public string roomName;
    public string hostName;
    public int playerCount;

    public RoomInfo(string roomName, string hostName, int playerCount)
    {
        this.roomName = roomName;
        this.hostName = hostName;
        this.playerCount = playerCount;
    }
}
