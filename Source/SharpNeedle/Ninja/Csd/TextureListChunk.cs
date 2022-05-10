namespace SharpNeedle.Ninja.Csd;
using System.IO;

public class TextureListChunk : List<CsdTexture>, IChunk
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("NXTL");
    public uint Signature { get; private set; } = BinSignature;
    public uint FieldC { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        options.Header ??= reader.ReadObject<ChunkHeader>();
        Signature = options.Header.Value.Signature;

        Clear();
        reader.ReadOffset(() =>
        {
            AddRange(reader.ReadObject<BinaryList<CsdTexture>>());
            var memoryTextureCount = 0;
            foreach (var texture in this)
            {
                if (texture.Name == null)
                    memoryTextureCount = Math.Max(texture.MemoryDataIndex + 1, memoryTextureCount);
            }

            if (memoryTextureCount > 0)
            {
                var textures = new byte[memoryTextureCount][];
                reader.ReadOffset(() =>
                {
                    for (int i = 0; i < memoryTextureCount; i++)
                        textures[i] = reader.ReadArrayOffset<byte>((int)reader.ReadOffsetValue());
                });

                foreach (var texture in this)
                {
                    if (texture.Name == null)
                        texture.Data = textures[texture.MemoryDataIndex];
                }
            }
        });
        FieldC = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions context)
    {
        writer.WriteLittle(Signature);
        writer.Write<int>(0);

        var start = writer.At();
        writer.Write(0x10); // List offset, untracked
        writer.Write(FieldC);
        var memoryTextureCount = 0;
        foreach (var texture in this)
        {
            if (texture.Name == null)
                texture.MemoryDataIndex = memoryTextureCount++;
        }

        writer.WriteObject<BinaryList<CsdTexture>>(this);
        if (memoryTextureCount > 0)
        {
            writer.WriteOffset(() =>
            {
                foreach (var texture in this)
                {
                    if (texture.Name != null)
                        continue;

                    writer.Write(texture.Data.Length);
                    writer.WriteArrayOffset(texture.Data);
                }
            });
        }

        writer.Flush();
        writer.Align(16);
        var end = writer.At();

        var size =         (long)end - (long)start;
        writer.At((long)start - sizeof(int), SeekOrigin.Begin);
        writer.Write((int)size);

        end.Dispose();
    }
}