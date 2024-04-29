using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ePacketID : UInt16
{
    CHAT_MESSAGE = 0,
    ROOMS_MESSAGE = 1,
    CONTENT_MESSAGE = 2,
    ENTER_ROOM = 3,
    LOGIN_SUCCESS = 4
}

[Serializable]
public struct PacketHeader
{
    public UInt16 size;
    public UInt16 id; // 프로토콜ID (ex. 1=로그인, 2=이동요청)
}
