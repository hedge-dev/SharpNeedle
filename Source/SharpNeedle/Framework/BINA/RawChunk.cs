﻿namespace SharpNeedle.Framework.BINA;

public class RawChunk : IChunk
{
    public uint Signature { get; set; }
    public byte[] Data { get; set; }

    public void Read(BinaryObjectReader reader, ChunkParseOptions options)
    {
        options.Header ??= reader.ReadObject<ChunkHeader>();
        Data = reader.ReadArray<byte>((int)options.Header.Value.Size - ChunkHeader.BinarySize);
    }

    public void Write(BinaryObjectWriter writer, ChunkParseOptions options)
    {
        writer.Write(Signature);
        writer.Write(Data.Length + ChunkHeader.BinarySize);
        writer.WriteArray(Data);
    }

    public T Parse<T>() where T : IBinarySerializable, new()
    {
        using MemoryStream stream = new(Data, false);
        using BinaryObjectReader reader = new(stream, StreamOwnership.Retain, Endianness.Little);
        return reader.ReadObject<T>();
    }
}