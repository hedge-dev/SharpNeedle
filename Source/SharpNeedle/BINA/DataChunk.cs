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

        var begin = writer.Position;
        writer.Write(Signature);
        
        var headToken = writer.At();
        writer.Write(0); // Size
        writer.Write(0); // String Table
        writer.Write(4); // String Table Size
        writer.Write(0); // Offset Table Size

        writer.Write((short)0x18);
        writer.Write((short)0x00);
        
        // Reserved
        writer.WriteCollection(Enumerable.Repeat<long>(0, 3));
        
        writer.PushOffsetOrigin();
        Data.Write(writer);
        writer.Flush();

        Offsets ??= new OffsetTable();
        Offsets.Clear();

        var origin = writer.OffsetHandler.OffsetOrigin;
        foreach (var offset in writer.OffsetHandler.OffsetPositions)
        {
            Offsets.Add(offset - origin);
        }

        var encodedOffsets = Offsets.Encode();

        var endToken = writer.At(writer.Length, SeekOrigin.End);
        headToken.Dispose();
        writer.Skip(4); // Size isn't ready yet
        writer.WriteOffset(() =>
        {
            writer.Write(0); // Empty string table
            writer.WriteArray(encodedOffsets);
        });
        writer.Skip(4); // We already wrote string table size
        writer.Write(encodedOffsets.Length);

        // Return to where we were
        endToken.Dispose();
        writer.Flush();

        endToken = writer.At(writer.Length, SeekOrigin.End);
        headToken.Dispose();
        writer.Write((uint)((long)endToken - begin));
        writer.PopOffsetOrigin();
    }
}

public class DataChunk
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("DATA");
    public static readonly uint AltBinSignature = BinaryHelper.MakeSignature<uint>("IMAG");

    public uint Signature { get; set; }
    public OffsetTable Offsets { get; set; }
}