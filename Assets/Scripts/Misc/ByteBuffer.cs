using System;
using System.Collections.Generic;
using System.Text;
// ReSharper disable All

public class ByteBuffer
{

    private const int maxCountNoStrip = 256;

    private List<byte> buffer;
    private byte[] readArray;
    private int readPosition;
    private bool bufferUpdated = false;

    public ByteBuffer()
    {
        buffer = new List<byte>();
        readPosition = 0;
    }

    public ByteBuffer(byte[] data)
    {
        buffer = new List<byte>(data);
        bufferUpdated = true;
        readPosition = 0;
    }

    private void CheckLength(int required = 0)
    {
        if (Length() < required)
            throw new Exception("ByteBuffer out of range");
    }

    private void UpdateBuffer()
    {
        if (bufferUpdated) {
            readArray = ToArray();
            bufferUpdated = false;
        }
    }

    public void Strip()
    {
        buffer.RemoveRange(0, readPosition);
        readPosition = 0;
    }

    public int GetReadPosition()
    {
        return readPosition;
    }

    public byte[] ToArray()
    {
        if (Count() > maxCountNoStrip)
            Strip();
        return buffer.ToArray();
    }

    public int Count()
    {
        return buffer.Count;
    }

    public int Length()
    {
        return Count() - readPosition;
    }

    public void Clear()
    {
        buffer.Clear();
        readPosition = 0;
        bufferUpdated = true;
    }

    public ByteBuffer WriteBytes(byte[] Input)
    {
        if (Count() > maxCountNoStrip)
            Strip();
        buffer.AddRange(Input);
        bufferUpdated = true;
        return this;
    }

    public ByteBuffer WriteByte(byte Input)
    {
        WriteBytes(new byte[] { Input });
        return this;
    }

    public ByteBuffer WriteBoolean(Boolean Input)
    {
        WriteBytes(BitConverter.GetBytes(Input));
        return this;
    }

    public ByteBuffer WriteSingle(Single Input)
    {
        WriteBytes(BitConverter.GetBytes(Input));
        return this;
    }

    public ByteBuffer WriteInt16(Int16 Input)
    {
        WriteBytes(BitConverter.GetBytes(Input));
        return this;
    }

    public ByteBuffer WriteInt32(Int32 Input)
    {
        WriteBytes(BitConverter.GetBytes(Input));
        return this;
    }

    public ByteBuffer WriteInt64(Int64 Input)
    {
        WriteBytes(BitConverter.GetBytes(Input));
        return this;
    }

    public ByteBuffer WriteChar(Char Input)
    {
        WriteBytes(BitConverter.GetBytes(Input));
        return this;
    }

    public ByteBuffer WriteString(String Input)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(Input);
        WriteInt32(bytes.Length);
        WriteBytes(bytes);
        return this;
    }

    public Boolean ReadBoolean(bool Peek = true)
    {
        int byteCount = 1;
        CheckLength(byteCount);
        UpdateBuffer();

        Boolean result = BitConverter.ToBoolean(readArray, readPosition);
        if (Peek)
            readPosition += byteCount;
        return result;
    }

    public Single ReadSingle(bool Peek = true)
    {
        int byteCount = 4;
        CheckLength(byteCount);
        UpdateBuffer();

        Single result = BitConverter.ToSingle(readArray, readPosition);
        if (Peek)
            readPosition += byteCount;
        return result;
    }

    public Int16 ReadInt16(bool Peek = true)
    {
        int byteCount = 2;
        CheckLength(byteCount);
        UpdateBuffer();

        Int16 result = BitConverter.ToInt16(readArray, readPosition);
        if (Peek)
            readPosition += byteCount;
        return result;
    }

    public Int32 ReadInt32(bool Peek = true)
    {
        int byteCount = 4;
        CheckLength(byteCount);
        UpdateBuffer();

        Int32 result = BitConverter.ToInt32(readArray, readPosition);
        if (Peek)
            readPosition += byteCount;
        return result;
    }

    public Int64 ReadInt64(bool Peek = true)
    {
        int byteCount = 8;
        CheckLength(byteCount);
        UpdateBuffer();

        Int64 result = BitConverter.ToInt64(readArray, readPosition);
        if (Peek)
            readPosition += byteCount;
        return result;
    }

    public Char ReadChar(bool Peek = true)
    {
        int byteCount = 1;
        CheckLength(byteCount);
        UpdateBuffer();

        Char result = BitConverter.ToChar(readArray, readPosition);
        if (Peek)
            readPosition += byteCount;
        return result;
    }

    public String ReadString(bool Peek = true)
    {
        int byteCount = ReadInt32(false) + 4;
        CheckLength(byteCount);
        UpdateBuffer();

        String result = Encoding.UTF8.GetString(readArray, readPosition + 4, byteCount - 4);
        if (Peek)
            readPosition += byteCount;
        return result;
    }

    public byte[] ReadBytes(int count, bool Peek = true)
    {
        UpdateBuffer();

        byte[] result = buffer.GetRange(readPosition, count).ToArray();
        if (Peek)
            readPosition += result.Length;
        return result;
    }

    public byte[] ReadBytes(bool Peek = true)
    {
        return ReadBytes(Length(), Peek);
    }

}