using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePacketID : UInt16
{
    CHAT_MESSAGE = 0,
    ROOMS_MESSAGE = 1,
    CONTENT_MESSAGE = 2
}

[Serializable]
public struct PacketHeader
{
    public UInt16 size;
    public UInt16 id; // ��������ID (ex. 1=�α���, 2=�̵���û)
}
