namespace SharpNeedle.BINA;
using System.Buffers.Binary;
using System.IO;

public abstract class BinaryResource : ResourceBase, IBinarySerializable
{
    public static readonly uint Signature = BinaryHelper.MakeSignature<uint>("BINA");

    public Version Version { get; set; }
    public uint Size { get; set; }
    public List<IChunk> Chunks { get; set; } = new(2);

    protected BinaryResource()
    {
        Version = new Version(2, 0, 0, BinaryHelper.PlatformEndianness);
        Chunks.Add(new DataChunk<IBinarySerializable>(this));
    }

    public override void Read(IFile file)
    {
        BaseFile = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);
        using var reader = new BinaryObjectReader(file.Open(), StreamOwnership.Transfer, Endianness.Little);

        Chunks.Clear();
        var signature = reader.ReadNative<uint>();
        var isV2 = BinaryHelper.EnsureSignature(signature, false, Signature);
        if (isV2)
        {
            Version = reader.ReadObject<Version>();
            reader.Endianness = Version.Endianness;

            Size = reader.Read<uint>();
            var chunkCount = reader.Read<ushort>();
            reader.Skip(2);

            var options = new ChunkParseOptions()
            {
                Owner = this
            };

            for (int i = 0; i < chunkCount; i++)
            {
                var header = reader.ReadObject<ChunkHeader>();
                options.Header = header;

                if (header.Signature == DataChunk.BinSignature || header.Signature == DataChunk.AltBinSignature)
                {
                    var chunk = new DataChunk<IBinarySerializable>(this);
                    chunk.Read(reader, options);
                    Chunks.Add(chunk);
                    continue;
                }

                Chunks.Add(reader.ReadObject<RawChunk, ChunkParseOptions>(options));
            }
        }
        else
        {
            var offTableOffset = reader.ReadNative<int>();
            var offTableSize = reader.ReadNative<int>();
            reader.Skip(8);

            Version = reader.ReadObject<Version>();
            reader.EnsureSignatureNative(Signature);
            reader.Skip(4); // Alignment?
            reader.PushOffsetOrigin();

            reader.Endianness = Version.Endianness;

            if (Version.Endianness != BinaryHelper.PlatformEndianness)
            {
                offTableOffset = BinaryPrimitives.ReverseEndianness(offTableOffset);
                offTableSize = BinaryPrimitives.ReverseEndianness(offTableSize);
                Size = BinaryPrimitives.ReverseEndianness(signature);
            }
            
            var chunk = new DataChunk<IBinarySerializable>(this)
            {
                Offsets = OffsetTable.FromBytes(reader.ReadArrayAtOffset<byte>(offTableOffset, offTableSize))
            };

            chunk.Read(reader, new() { Owner = this });
            Chunks.Add(chunk);
        }
    }

    public override void Write(IFile file)
    {
        BaseFile = file;
        using var writer = new BinaryObjectWriter(file.Open(FileAccess.Write), StreamOwnership.Transfer, Version.Endianness);
        if (Version.IsV1)
            WriteV1(writer);
        else
            WriteV2(writer);
    }

    private void WriteV1(BinaryObjectWriter writer)
    {
        var begin = writer.Position;
        var sizeToken = writer.At();
        writer.Write(0);
        writer.Write(0L); // Offset table
        writer.Write(0L); // what

        writer.WriteObject(Version);
        writer.Write(Signature);
        writer.Write(0);
        writer.PushOffsetOrigin();

        var options = new ChunkParseOptions
        {
            Owner = this
        };

        foreach (var chunk in Chunks)
            chunk.Write(writer, options);

        writer.Flush();
        var endToken = writer.At(writer.Length, SeekOrigin.End);
        sizeToken.Dispose();

        var offTable = new OffsetTable();
        var origin = writer.OffsetHandler.OffsetOrigin;
        foreach (var offset in writer.OffsetHandler.OffsetPositions)
        {
            offTable.Add(offset - origin);
        }

        var offTableEncoded = offTable.Encode();
        writer.Skip(4); // Skip size for now
        writer.WriteArrayOffset(offTableEncoded);
        writer.Write(offTableEncoded.Length);

        endToken.Dispose();
        writer.Flush();
        endToken = writer.At(writer.Length, SeekOrigin.End);

        sizeToken.Dispose();
        writer.Write((int)((long)endToken - begin));
        writer.PopOffsetOrigin();
        endToken.Dispose();
    }

    private void WriteV2(BinaryObjectWriter writer)
    {
        var begin = writer.Position;
        writer.Write(Signature);
        writer.WriteObject(Version);

        var sizeToken = writer.At();
        writer.Write(0);
        writer.Write((short)Chunks.Count);
        writer.Align(4);

        var options = new ChunkParseOptions
        {
            Owner = this
        };
        foreach (var chunk in Chunks)
            writer.WriteObject(chunk, options);

        using var end = writer.At(writer.Length, SeekOrigin.Begin);
        sizeToken.Dispose();

        writer.Write((int)((long)end - begin));
    }

    public abstract void Read(BinaryObjectReader reader);

    public abstract void Write(BinaryObjectWriter writer);
}