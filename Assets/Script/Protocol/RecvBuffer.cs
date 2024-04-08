using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecvBuffer
{
    private Int32 _capacity = 0;
    private Int32 _bufferSize = 0;
    private Int32 _readPos = 0;
    private Int32 _writePos = 0;
    private byte[] _buffer;

    public RecvBuffer(Int32 bufferSize)
    {
        _bufferSize = bufferSize;
        _capacity = bufferSize * 2;
        _buffer = new byte[_capacity];
    }

    public void Clean()
    {
        Int32 dataSize = DataSize();
        if (dataSize == 0)
        {
            // 딱 마침 읽기+쓰기 커서가 동일한 위치라면, 둘 다 리셋.
            _readPos = _writePos = 0;
        }
        else
        {
            // 여유 공간이 버퍼 1개 크기 미만이면, 데이터를 앞으로 땅긴다.
            if (FreeSize() < _bufferSize)
            {
                Buffer.BlockCopy(_buffer, _readPos, _buffer, 0, dataSize);
                _readPos = 0;
                _writePos = dataSize;
            }
        }
    }

    public bool OnRead(Int32 numOfBytes)
    {
        if (numOfBytes > DataSize())
            return false;

        _readPos += numOfBytes;
        return true;
    }

    public bool OnWrite(Int32 numOfBytes)
    {
        if (numOfBytes > FreeSize())
            return false;

        _writePos += numOfBytes;
        return true;
    }

    public Span<byte> ReadPos()
    {
        return _buffer.AsSpan(_readPos, _bufferSize);
    }

    public Span<byte> WritePos()
    {
        return _buffer.AsSpan(_writePos, _bufferSize);
    }

    public Int32 DataSize()
    {
        return _writePos - _readPos;
    }

    public Int32 FreeSize()
    {
        return _capacity - _writePos;
    }
}
