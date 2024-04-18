using Google.Protobuf;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;


public class PacketHandler
{
    public static byte[] SerializePacket<T>(T pPkt, ePacketID pMessageID) where T : IMessage
    {
        var s = pPkt.CalculateSize() + Marshal.SizeOf(typeof(PacketHeader));
        PacketHeader header = new PacketHeader();
        header.size = (ushort)s;
        header.id = (UInt16)pMessageID;

        using (MemoryStream ret = new MemoryStream())
        {
            using (BinaryWriter writer = new BinaryWriter(ret, Encoding.Default, true))
            {
                //패킷 헤더 직렬화
                writer.Write(header.size);
                writer.Write(header.id);
            }
            pPkt.WriteTo(ret);

            byte[] arr = ret.ToArray();
            var l = arr.Length;
            return ret.ToArray();
        }

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

        byte[] arr = ret.ToArray();
        var l = arr.Length;
        return arr;
    }
}
