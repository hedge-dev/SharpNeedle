namespace SharpNeedle.Ninja;
using System.IO;
using Cell;

public class InfoChunk : IChunk
{
    public uint Signature { get; private set; }
    public List<IChunk> Chunks { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        options.Header ??= reader.ReadObject<ChunkHeader>();
        Signature = options.Header.Value.Signature;
        var chunkCount = reader.Read<int>();
        
        Chunks = new List<IChunk>(chunkCount);
        reader.ReadOffset(() =>
        {
            reader.PushOffsetOrigin();
            for (int i = 0; i < chunkCount; i++)
            {
                var header = reader.ReadObject<ChunkHeader>();
                var begin = reader.Position;
                options.Header = header;
                if (header.Signature == ProjectChunk.BinSignature)
                    Chunks.Add(reader.ReadObject<ProjectChunk, ChunkBinaryOptions>(options));

                reader.At(begin + header.Size, SeekOrigin.Begin);
            }
            reader.PopOffsetOrigin();
        });
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        throw new NotImplementedException();
    }
}