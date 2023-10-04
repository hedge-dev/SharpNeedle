namespace SharpNeedle.BINA;
using System.IO;

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

        var strTableOffset = reader.Read<uint>();
        var strTableSize = reader.Read<uint>();
        var offTableSize = reader.Read<int>();

        var dataOffset = reader.Read<ushort>();
        reader.Skip(2);

        reader.Skip(dataOffset);
        reader.PushOffsetOrigin();
        Data.Read(reader);

        reader.ReadAtOffset(strTableOffset + strTableSize, () =>
        {
            Offsets = OffsetTable.FromBytes(reader.ReadArray<byte>(offTableSize));
        });
    }

    public void Write(BinaryObjectWriter writer, ChunkParseOptions options)
    {
        if (options.Owner.Version.IsV1)
        {
            Data.Write(writer);
            return;
        }
        
        var dataStream = new MemoryStream();
        var dataWriter = new BinaryObjectWriter(dataStream, StreamOwnership.Transfer, writer.Endianness, writer.Encoding, writer.FilePath);
        dataWriter.OffsetBinaryFormat = writer.OffsetBinaryFormat;
        Data.Write(dataWriter);
        dataWriter.Flush();

        Offsets = new OffsetTable();
        foreach (var offset in dataWriter.OffsetHandler.OffsetPositions)
        {
            Offsets.Add(offset);
        }

        dataWriter.Seek(0, SeekOrigin.End);
        dataWriter.Align(4);

        var baseSize = dataWriter.Length;
        var table = Offsets.Encode();
        dataWriter.Write(0); // Dummy string table
        dataWriter.WriteArray(table.AsSpan());

        writer.Write(Signature);
        
        writer.Write((int)(baseSize + table.Length + 0x34)); // Size
        writer.Write((int)baseSize); // String Table
        writer.Write(4); // String Table Size
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
    public OffsetTable Offsets { get; set; }
}