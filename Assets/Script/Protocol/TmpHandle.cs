using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using UnityEngine;

public class TmpHandle
{
    public byte[] MakeBuffer(ePacketID pMessageID)
    {
        byte[] ret;

        try
        {
            PacketHeader packetHeader = new PacketHeader();
            packetHeader.size = (UInt16)Marshal.SizeOf(typeof(PacketHeader));
            packetHeader.id = (UInt16)pMessageID;

            using (var memoryStream = new MemoryStream())
            {
                // PacketHeader를 바이트 배열로 변환하여 MemoryStream에 쓰기
                using (var writer = new BinaryWriter(memoryStream, Encoding.Default, true))
                {
                    writer.Write(packetHeader.size);
                    writer.Write(packetHeader.id);
                }

                // 최종 바이트 배열 가져오기
                byte[] finalBytes = memoryStream.ToArray();

                await _stream.WriteAsync(finalBytes, 0, finalBytes.Length);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while sending data: " + e.Message);
        }
        return ret;
    }
}
