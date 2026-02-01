namespace SharpNeedle.Framework.SurfRide;

using SharpNeedle.Framework.SurfRide.Draw;

public class InfoChunk : IChunk
{
    public uint Signature { get; private set; } = BinaryHelper.MakeSignature<uint>("SWIF");
    public List<IChunk> Chunks { get; set; } = [];
    public OffsetChunk Offsets { get; set; } = [];
    public int Version { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        options.Header ??= reader.ReadObject<ChunkHeader>();
        reader.Endianness = (options.Header.Value.Size & 0xFFFF0000) != 0 ? Endianness.Big : Endianness.Little;
        Signature = options.Header.Value.Signature;
        int chunkCount = reader.ReadInt32();

        SeekToken beforeChunk = reader.At();
        reader.Skip(12);
        Version = reader.ReadInt32();
        if (options.Version <= 2)
        {
            switch (Version)
            {
                case 20100420:
                    options.Version = 0;
                    break;
                case 20101207:
                    options.Version = 1;
                    break;
                case 20120705:
                    options.Version = 2;
                    break;
                default:
                    break;
            }
        }

        beforeChunk.Dispose();

        Chunks = new List<IChunk>(chunkCount);
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < chunkCount; i++)
            {
                reader.OffsetBinaryFormat = OffsetBinaryFormat.U32;
                ChunkHeader header = reader.ReadObject<ChunkHeader>();
                long begin = reader.Position;
                options.Header = header;
                if (header.Signature == ProjectChunk.BinSignature)
                {
                    Chunks.Add(reader.ReadObject<ProjectChunk, ChunkBinaryOptions>(options));
                }
                else if (header.Signature == TextureListChunk.BinSignature)
                {
                    Chunks.Add(reader.ReadObject<TextureListChunk, ChunkBinaryOptions>(options));
                }

                reader.At(begin + header.Size, SeekOrigin.Begin);
            }
        });

        reader.Skip(4); // size of all chunks
        Offsets = reader.ReadObjectOffset<OffsetChunk>();
        reader.Skip(4); // date has already been read
        reader.Skip(4); // skip padding
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.WriteLittle(Signature);
        writer.WriteInt32(0x18); // Always constant
        writer.WriteInt32(Chunks.Count);
        writer.WriteInt32(0x20); // Always at the end of the chunk

        SeekToken sizePos = writer.At();
        writer.WriteInt32(0); // Chunk list size

        writer.WriteInt32(0); // OffsetChunk Ptr
        writer.WriteInt32(Version);
        writer.WriteInt32(0); // Padding

        SeekToken chunkBegin = writer.At();
        foreach (IChunk chunk in Chunks)
        {
            writer.WriteObject(chunk, options);
        }

        SeekToken chunkEnd = writer.At();

        Offsets = [];
        foreach (long offset in writer.OffsetHandler.OffsetPositions)
        {
            Offsets.Add((int)offset);
        }

        SeekToken offsetChunkPos = writer.At();
        writer.WriteObject(Offsets);
        {
            writer.WriteNative(BinaryHelper.MakeSignature<uint>("SEND"));
            writer.WriteInt32(0);
            writer.WriteInt64(0);
        }

        sizePos.Dispose();
        writer.WriteInt32((int)chunkEnd - (int)chunkBegin);
        writer.WriteInt32((int)offsetChunkPos);
        writer.Flush();
    }
}
