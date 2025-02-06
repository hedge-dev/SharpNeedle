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
        int chunkCount = reader.Read<int>();

        SeekToken beforeChunk = reader.At();
        reader.Skip(12);
        Version = reader.Read<int>();
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
        writer.Write(0x18); // Always constant
        writer.Write(Chunks.Count);
        writer.Write(0x20); // Always at the end of the chunk

        SeekToken sizePos = writer.At();
        writer.Write(0); // Chunk list size

        writer.Write(0); // OffsetChunk Ptr
        writer.Write(Version);
        writer.Write(0); // Padding

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
            writer.Write(0);
            writer.Write<long>(0);
        }

        sizePos.Dispose();
        writer.Write((int)chunkEnd - (int)chunkBegin);
        writer.Write((int)offsetChunkPos);
        writer.Flush();
    }
}
