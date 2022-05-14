namespace SharpNeedle.Ninja.Csd;
using System.IO;

public class TextureListDXL : ITextureList
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("NXTL");
    public uint Signature { get; private set; } = BinSignature;
    public uint FieldC { get; set; }
    public List<TextureDXL> Textures { get; set; } = new();
    public int Count => Textures.Count;
    public bool IsReadOnly => false;

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        options.Header ??= reader.ReadLittle<ChunkHeader>();
        Signature = options.Header.Value.Signature;

        Textures.Clear();
        reader.ReadOffset(() =>
        {
            Textures.AddRange(reader.ReadObject<BinaryList<TextureDXL>>());
            var memoryTextureCount = 0;
            foreach (var texture in Textures)
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

                foreach (var texture in Textures)
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
        foreach (var texture in Textures)
        {
            if (texture.Name == null)
                texture.MemoryDataIndex = memoryTextureCount++;
        }

        writer.WriteObject<BinaryList<TextureDXL>>(Textures);
        if (memoryTextureCount > 0)
        {
            writer.WriteOffset(() =>
            {
                foreach (var texture in Textures)
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

    public void Add(ITexture item)
    {
        Textures.Add((TextureDXL)item);
    }

    public void Clear()
    {
        Textures.Clear();
    }

    public bool Contains(ITexture item)
    {
        return Textures.Contains((TextureDXL)item);
    }

    public void CopyTo(ITexture[] array, int arrayIndex)
    {
        for (int i = 0; i < Count; i++)
            array[arrayIndex + i] = Textures[i];
    }

    public bool Remove(ITexture item)
    {
        return Textures.Remove((TextureDXL)item);
    }
    
    public int IndexOf(ITexture item)
    {
        return Textures.IndexOf((TextureDXL)item);
    }

    public void Insert(int index, ITexture item)
    {
        Textures.Insert(index, (TextureDXL)item);
    }

    public void RemoveAt(int index)
    {
        Textures.RemoveAt(index);
    }

    public ITexture this[int index]
    {
        get => Textures[index];
        set => Textures[index] = (TextureDXL)value;
    }

    public IEnumerator<ITexture> GetEnumerator()
    {
        return Textures.Cast<ITexture>().GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}