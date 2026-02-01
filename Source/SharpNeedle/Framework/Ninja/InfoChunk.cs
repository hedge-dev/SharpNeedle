namespace SharpNeedle.Framework.Ninja;

using SharpNeedle.Framework.Ninja.Csd;

public class InfoChunk : IChunk
{
    public uint Signature { get; set; } = BinaryHelper.MakeSignature<uint>("NXIF");
    public List<IChunk> Chunks { get; set; } = [];
    public OffsetChunk Offsets { get; set; } = [];
    public uint Field1C { get; set; } = 1;

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        options.Header ??= reader.ReadObject<ChunkHeader>();
        Signature = options.Header.Value.Signature;
        int chunkCount = reader.ReadInt32();

        // Check if Signature ends with IF
        if (((Signature >> 16) & 0xFFFF) != 0x4649)
        {
            throw new InvalidDataException($"Invalid Signature: 0x{Signature:X}");
        }

        Chunks.Clear();
        Chunks.Capacity = chunkCount;
        reader.ReadOffset(() =>
        {
            reader.PushOffsetOrigin();
            for (int i = 0; i < chunkCount; i++)
            {
                ChunkHeader header = reader.ReadObject<ChunkHeader>();
                long begin = reader.Position;
                options.Header = header;
                if (header.Signature == ProjectChunk.BinSignature)
                {
                    Chunks.Add(reader.ReadObject<ProjectChunk, ChunkBinaryOptions>(options));
                }
                else if (((Signature >> 16) & 0xFFFF) != 0x4C54) // TL
                {
                    switch (options.TextureFormat)
                    {
                        case TextureFormat.NextNinja:
                            Chunks.Add(reader.ReadObject<TextureListNN, ChunkBinaryOptions>(options));
                            break;

                        default:
                            Chunks.Add(reader.ReadObject<TextureListMirage, ChunkBinaryOptions>(options));
                            break;
                    }
                }

                reader.At(begin + header.Size, SeekOrigin.Begin);
            }

            reader.PopOffsetOrigin();
        });

        reader.Skip(4); // size of all chunks
        Offsets = reader.ReadObjectOffset<OffsetChunk>();
        reader.Skip(4); // Offsets.BinarySize

        Field1C = reader.ReadUInt32();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.WriteLittle(Signature);
        writer.WriteLittle(0x18); // Always constant
        writer.WriteInt32(Chunks.Count);

        writer.WriteInt32(0x20); // Always at the end of the chunk

        SeekToken sizePos = writer.At();
        writer.WriteInt32(0); // Chunk list size

        writer.WriteInt32(0); // OffsetChunk Ptr
        writer.WriteInt32(0); // OffsetChunk.BinarySize
        writer.WriteUInt32(Field1C);

        writer.PushOffsetOrigin();
        long offsetBase = writer.OffsetHandler.OffsetOrigin;

        SeekToken chunkBegin = writer.At();
        foreach (IChunk chunk in Chunks)
        {
            writer.WriteObject(chunk, options);
        }

        SeekToken chunkEnd = writer.At();
        writer.PopOffsetOrigin();

        Offsets = [];
        foreach (long offset in writer.OffsetHandler.OffsetPositions)
        {
            Offsets.Add((int)(offset - offsetBase));
        }

        SeekToken offsetChunkPos = writer.At();
        writer.WriteObject(Offsets);
        {
            writer.WriteNative(BinaryHelper.MakeSignature<uint>("NEND"));
            writer.WriteInt32(0);
            writer.WriteInt64(0L);
        }

        sizePos.Dispose();
        writer.WriteInt32((int)chunkEnd - (int)chunkBegin);
        writer.WriteInt32((int)offsetChunkPos);
        writer.WriteInt32(Offsets.BinarySize);
        writer.Flush();
    }
}