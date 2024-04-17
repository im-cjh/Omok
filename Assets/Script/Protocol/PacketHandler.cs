using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;


public class PacketHandler
{
    public static byte[] SerializePacket<T>(T pPkt, ePacketID pMessageID) where T : IMessage
    {
        PacketHeader header = new PacketHeader();
        header.size = (UInt16)Marshal.SizeOf(typeof(PacketHeader));
        header.id = (UInt16)pMessageID;

        MemoryStream ret = new MemoryStream();

        using (BinaryWriter writer = new BinaryWriter(ret))
        {
            //패킷 헤더 직렬화
            writer.Write(header.size);
            writer.Write(header.id);
        }
        pPkt.WriteTo(ret);
        
        return ret.GetBuffer();
    }

    public static byte[] SerializeHeader(ePacketID pMessageID)
    {
        PacketHeader header = new PacketHeader();
        header.size = (UInt16)Marshal.SizeOf(typeof(PacketHeader));
        header.id = (UInt16)pMessageID;

        MemoryStream ret = new MemoryStream();

        using (BinaryWriter writer = new BinaryWriter(ret))
        {
            //패킷 헤더 직렬화
            writer.Write(header.size);
            writer.Write(header.id);
        }

        return ret.GetBuffer();
    }
}
