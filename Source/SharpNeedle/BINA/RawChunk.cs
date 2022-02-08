namespace SharpNeedle.BINA;
using System.IO;

public class RawChunk : IChunk
{
    public uint Signature { get; set; }
    public byte[] Data { get; set; }

    public void Read(BinaryObjectReader reader, ChunkHeader? type)
    {
        type ??= reader.ReadObject<ChunkHeader>();
        Data = reader.ReadArray<byte>((int)type.Value.Size - ChunkHeader.BinarySize);
    }

    public void Write(BinaryObjectWriter writer, ChunkHeader? header)
    {
        writer.Write(Signature);
        writer.Write(Data.Length + ChunkHeader.BinarySize);
        writer.WriteArray(Data);
    }

    public T Parse<T>() where T : IBinarySerializable, new()
    {
        using var stream = new MemoryStream(Data, false);
        using var reader = new BinaryObjectReader(stream, StreamOwnership.Retain, Endianness.Little);
        return reader.ReadObject<T>();
    }
}