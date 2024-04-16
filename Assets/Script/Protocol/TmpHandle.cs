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
                // PacketHeader�� ����Ʈ �迭�� ��ȯ�Ͽ� MemoryStream�� ����
                using (var writer = new BinaryWriter(memoryStream, Encoding.Default, true))
                {
                    writer.Write(packetHeader.size);
                    writer.Write(packetHeader.id);
                }

                // ���� ����Ʈ �迭 ��������
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
