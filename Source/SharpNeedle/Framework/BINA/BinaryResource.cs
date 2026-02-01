namespace SharpNeedle.Framework.BINA;

using System.Buffers.Binary;

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
        using BinaryObjectReader reader = new(file.Open(), StreamOwnership.Transfer, Endianness.Little);

        Chunks.Clear();
        uint signature = reader.ReadNative<uint>();
        bool isV2 = BinaryHelper.EnsureSignature(signature, false, Signature);
        if (isV2)
        {
            Version = reader.ReadObject<Version>();
            if (Version.Is64Bit)
            {
                reader.OffsetBinaryFormat = OffsetBinaryFormat.U64;
            }

            reader.Endianness = Version.Endianness;

            Size = reader.ReadUInt32();
            ushort chunkCount = reader.ReadUInt16();
            reader.Skip(2);

            ChunkParseOptions options = new()
            {
                Owner = this
            };

            for (int i = 0; i < chunkCount; i++)
            {
                ChunkHeader header = reader.ReadObject<ChunkHeader>();
                options.Header = header;

                if (header.Signature == DataChunk.BinSignature || header.Signature == DataChunk.AltBinSignature)
                {
                    DataChunk<IBinarySerializable> chunk = new(this);
                    chunk.Read(reader, options);
                    Chunks.Add(chunk);
                    continue;
                }

                Chunks.Add(reader.ReadObject<RawChunk, ChunkParseOptions>(options));
            }
        }
        else
        {
            int offTableOffset = reader.ReadNative<int>();
            int offTableSize = reader.ReadNative<int>();
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

            DataChunk<IBinarySerializable> chunk = new(this)
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
        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Version.Endianness);
        if (Version.Is64Bit)
        {
            writer.OffsetBinaryFormat = OffsetBinaryFormat.U64;
        }

        if (Version.IsV1)
        {
            WriteV1(writer);
        }
        else
        {
            WriteV2(writer);
        }
    }

    private void WriteV1(BinaryObjectWriter writer)
    {
        long begin = writer.Position;
        SeekToken sizeToken = writer.At();
        writer.WriteInt32(0);
        writer.WriteInt64(0); // Offset table
        writer.WriteInt64(0); // what

        writer.WriteObject(Version);
        writer.WriteUInt32(Signature);
        writer.WriteInt32(0);
        writer.PushOffsetOrigin();

        ChunkParseOptions options = new()
        {
            Owner = this
        };

        foreach (IChunk chunk in Chunks)
        {
            chunk.Write(writer, options);
        }

        writer.Flush();
        SeekToken endToken = writer.At(writer.Length, SeekOrigin.End);
        sizeToken.Dispose();

        OffsetTable offTable = [];
        long origin = writer.OffsetHandler.OffsetOrigin;
        foreach (long offset in writer.OffsetHandler.OffsetPositions)
        {
            offTable.Add(offset - origin);
        }

        byte[] offTableEncoded = offTable.Encode();
        writer.Skip(4); // Skip size for now
        writer.WriteArrayOffset(offTableEncoded);
        writer.WriteInt32(offTableEncoded.Length);

        endToken.Dispose();
        writer.Flush();
        endToken = writer.At(writer.Length, SeekOrigin.End);

        sizeToken.Dispose();
        writer.WriteInt32((int)((long)endToken - begin));
        writer.PopOffsetOrigin();
        endToken.Dispose();
    }

    private void WriteV2(BinaryObjectWriter writer)
    {
        long begin = writer.Position;
        writer.WriteNative(Signature);
        writer.WriteObject(Version);

        SeekToken sizeToken = writer.At();
        writer.WriteInt32(0);
        writer.WriteInt16((short)Chunks.Count);
        writer.Align(4);

        ChunkParseOptions options = new()
        {
            Owner = this
        };
        foreach (IChunk chunk in Chunks)
        {
            writer.WriteObject(chunk, options);
        }

        using SeekToken end = writer.At(writer.Length, SeekOrigin.Begin);
        sizeToken.Dispose();

        writer.WriteInt32((int)((long)end - begin));
    }

    public abstract void Read(BinaryObjectReader reader);

    public abstract void Write(BinaryObjectWriter writer);
}