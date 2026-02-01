namespace SharpNeedle.Framework.SurfRide.Draw;

public class TextureListChunk : List<TextureList>, IChunk
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("SWTL");
    public uint Signature { get; private set; } = BinSignature;

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        SeekToken start = reader.At();
        options.Header ??= reader.ReadObject<ChunkHeader>();
        Signature = options.Header.Value.Signature;
        int listOffset = reader.ReadInt32();
        Capacity = reader.ReadInt32();
        reader.ReadAtOffset((int)start - 8 + listOffset, () => AddRange(reader.ReadObjectArray<TextureList, ChunkBinaryOptions>(options, Capacity)));
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.WriteLittle(Signature);
        writer.WriteInt32(0); // Size

        SeekToken start = writer.At();
        writer.WriteInt32(0x10); // List Offset, untracked
        writer.WriteInt32(Count);

        writer.WriteObjectCollection(options, this);

        writer.Flush();
        writer.Align(16);
        SeekToken end = writer.At();

        long size = (long)end - (long)start;
        writer.At((long)start - sizeof(int), SeekOrigin.Begin);
        writer.WriteInt32((int)size);

        end.Dispose();
    }

    public TextureList? GetTextureList(string name)
    {
        return Find(item => item.Name == name);
    }
}

public class TextureList : List<Texture>, IBinarySerializable<ChunkBinaryOptions>
{
    public string? Name { get; set; }
    public uint Field08 { get; set; }
    public uint Field14 { get; set; }
    public UserData? UserData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        if (options.Version >= 3)
        {
            reader.OffsetBinaryFormat = OffsetBinaryFormat.U64;
        }

        Name = reader.ReadStringOffset();
        if (options.Version >= 4)
        {
            Field08 = reader.ReadUInt32();
        }

        Capacity = reader.ReadInt32();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        AddRange(reader.ReadObjectArrayOffset<Texture, ChunkBinaryOptions>(options, Capacity));
        long userDataOffset = reader.ReadOffsetValue();
        if (userDataOffset != 0)
        {
            UserData = reader.ReadObjectAtOffset<UserData, ChunkBinaryOptions>(userDataOffset, options);
        }

        if (options.Version == 0)
        {
            Field14 = reader.ReadUInt32();
        }
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
        {
            writer.OffsetBinaryFormat = OffsetBinaryFormat.U64;
            writer.DefaultAlignment = 8;
        }

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        if (options.Version >= 4)
        {
            writer.WriteUInt32(Field08);
        }

        writer.WriteInt32(Count);
        writer.WriteObjectCollectionOffset(options, this);
        if (UserData != null)
        {
            writer.WriteObjectOffset(UserData, options);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        if (options.Version == 0)
        {
            writer.WriteUInt32(Field14);
        }
    }

    public Texture? GetTexture(string name)
    {
        return Find(item => item.Name == name);
    }
}