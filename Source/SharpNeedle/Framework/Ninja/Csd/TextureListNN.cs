namespace SharpNeedle.Framework.Ninja.Csd;

public class TextureListNN : ITextureList
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("NGTL");
    public uint Signature { get; private set; } = BinSignature;
    public uint FieldC { get; set; }
    public List<TextureNN> Textures { get; set; } = [];
    public int Count => Textures.Count;
    public bool IsReadOnly => false;

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        options.Header ??= reader.ReadLittle<ChunkHeader>();
        Signature = options.Header.Value.Signature;

        Textures.Clear();
        reader.ReadOffset(() => Textures.AddRange(reader.ReadObject<BinaryList<TextureNN>>()));
        FieldC = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions context)
    {
        writer.WriteLittle(Signature);
        writer.Write(0);

        SeekToken start = writer.At();
        writer.Write(0x10); // List offset, untracked
        writer.Write(FieldC);

        writer.WriteObject<BinaryList<TextureNN>>(Textures);

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
        Textures.Add((TextureNN)item);
    }

    public void Clear()
    {
        Textures.Clear();
    }

    public bool Contains(ITexture item)
    {
        return Textures.Contains((TextureNN)item);
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
        return Textures.Remove((TextureNN)item);
    }

    public int IndexOf(ITexture item)
    {
        return Textures.IndexOf((TextureNN)item);
    }

    public void Insert(int index, ITexture item)
    {
        Textures.Insert(index, (TextureNN)item);
    }

    public void RemoveAt(int index)
    {
        Textures.RemoveAt(index);
    }

    public ITexture this[int index]
    {
        get => Textures[index];
        set => Textures[index] = (TextureNN)value;
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