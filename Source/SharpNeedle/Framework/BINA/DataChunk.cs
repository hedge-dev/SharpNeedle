namespace SharpNeedle.Framework.BINA;

public class DataChunk<T> : DataChunk, IChunk where T : IBinarySerializable
{
    public T Data { get; set; }

    public DataChunk(T data)
    {
        Data = data;
    }

    public void Read(BinaryObjectReader reader, ChunkParseOptions options)
    {
        if (options.Owner.Version.IsV1)
        {
            Data.Read(reader);
            return;
        }

        options.Header ??= reader.ReadObject<ChunkHeader>();

        Signature = options.Header.Value.Signature;
        BinaryHelper.EnsureSignature(Signature, true, BinSignature, AltBinSignature);

        uint strTableOffset = reader.Read<uint>();
        uint strTableSize = reader.Read<uint>();
        int offTableSize = reader.Read<int>();

        ushort dataOffset = reader.Read<ushort>();
        reader.Skip(2);

        reader.Skip(dataOffset);
        reader.PushOffsetOrigin();
        Data.Read(reader);

        reader.ReadAtOffset(strTableOffset + strTableSize, () => Offsets = OffsetTable.FromBytes(reader.ReadArray<byte>(offTableSize)));
    }

    public void Write(BinaryObjectWriter writer, ChunkParseOptions options)
    {
        if (options.Owner.Version.IsV1)
        {
            Data.Write(writer);
            return;
        }

        using MemoryStream dataStream = new();
        using BinaryObjectWriter dataWriter = new(dataStream, StreamOwnership.Transfer, writer.Endianness, writer.Encoding, writer.FilePath)
        {
            OffsetBinaryFormat = writer.OffsetBinaryFormat
        };

        Data.Write(dataWriter);
        dataWriter.Flush();

        Offsets = [.. dataWriter.OffsetHandler.OffsetPositions];

        dataWriter.Seek(0, SeekOrigin.End);
        dataWriter.Align(4);

        long baseSize = dataWriter.Length;
        byte[] table = Offsets.Encode();
        // string table would go here
        dataWriter.WriteArray(table.AsSpan());

        writer.WriteNative(Signature);

        writer.Write((int)(baseSize + table.Length + 0x34)); // Size
        writer.Write((int)baseSize); // String Table
        writer.Write(0); // String Table Size
        writer.Write(table.Length); // Offset Table Size

        writer.Write((short)0x18);
        writer.Write((short)0x00);

        // Reserved
        writer.WriteCollection(Enumerable.Repeat<long>(0, 3));

        // Slice the buffer because the capacity can be bigger
        writer.WriteArray(dataStream.GetBuffer().AsSpan()[..(int)dataStream.Length]);
    }
}

public class DataChunk
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("DATA");
    public static readonly uint AltBinSignature = BinaryHelper.MakeSignature<uint>("IMAG");

    public uint Signature { get; set; } = BinSignature;
    public OffsetTable? Offsets { get; set; }
}