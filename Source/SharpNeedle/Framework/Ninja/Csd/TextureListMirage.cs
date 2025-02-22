namespace SharpNeedle.Framework.Ninja.Csd;

public class TextureListMirage : ITextureList
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("NXTL");
    public uint Signature { get; private set; } = BinSignature;
    public uint FieldC { get; set; }
    public List<TextureMirage> Textures { get; set; } = [];
    public int Count => Textures.Count;
    public bool IsReadOnly => false;

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        options.Header ??= reader.ReadLittle<ChunkHeader>();
        Signature = options.Header.Value.Signature;

        Textures.Clear();
        reader.ReadOffset(() =>
        {
            Textures.AddRange(reader.ReadObject<BinaryList<TextureMirage>>());
            int memoryTextureCount = 0;
            foreach (TextureMirage texture in Textures)
            {
                if (texture.Name == null)
                {
                    memoryTextureCount = Math.Max(texture.MemoryDataIndex + 1, memoryTextureCount);
                }
            }

            if (memoryTextureCount > 0)
            {
                byte[][] textures = new byte[memoryTextureCount][];
                reader.ReadOffset(() =>
                {
                    for (int i = 0; i < memoryTextureCount; i++)
                    {
                        textures[i] = reader.ReadArrayOffset<byte>((int)reader.ReadOffsetValue());
                    }
                });

                foreach (TextureMirage texture in Textures)
                {
                    if (texture.Name == null)
                    {
                        texture.Data = textures[texture.MemoryDataIndex];
                    }
                }
            }
        });
        FieldC = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions context)
    {
        writer.WriteLittle(Signature);
        writer.Write(0);

        SeekToken start = writer.At();
        writer.Write(0x10); // List offset, untracked
        writer.Write(FieldC);
        int memoryTextureCount = 0;
        foreach (TextureMirage texture in Textures)
        {
            if (texture.Name == null)
            {
                texture.MemoryDataIndex = memoryTextureCount++;
            }
        }

        writer.WriteObject<BinaryList<TextureMirage>>(Textures);
        if (memoryTextureCount > 0)
        {
            writer.WriteOffset(() =>
            {
                foreach (TextureMirage texture in Textures)
                {
                    if (texture.Name != null)
                    {
                        continue;
                    }

                    writer.Write(texture.Data.Length);
                    writer.WriteArrayOffset(texture.Data);
                }
            });
        }

        writer.Flush();
        writer.Align(16);
        SeekToken end = writer.At();

        long size = (long)end - (long)start;
        writer.At((long)start - sizeof(int), SeekOrigin.Begin);
        writer.WriteLittle((int)size);

        end.Dispose();
    }

    public void Add(ITexture item)
    {
        Textures.Add((TextureMirage)item);
    }

    public void Clear()
    {
        Textures.Clear();
    }

    public bool Contains(ITexture item)
    {
        return Textures.Contains((TextureMirage)item);
    }

    public void CopyTo(ITexture[] array, int arrayIndex)
    {
        for (int i = 0; i < Count; i++)
        {
            array[arrayIndex + i] = Textures[i];
        }
    }

    public bool Remove(ITexture item)
    {
        return Textures.Remove((TextureMirage)item);
    }

    public int IndexOf(ITexture item)
    {
        return Textures.IndexOf((TextureMirage)item);
    }

    public void Insert(int index, ITexture item)
    {
        Textures.Insert(index, (TextureMirage)item);
    }

    public void RemoveAt(int index)
    {
        Textures.RemoveAt(index);
    }

    public ITexture this[int index]
    {
        get => Textures[index];
        set => Textures[index] = (TextureMirage)value;
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