namespace SharpNeedle.SurfRide.Draw;
using System.IO;

public class TextureListChunk : List<TextureList>, IChunk
{
    public static readonly uint BinSignature = BinaryHelper.MakeSignature<uint>("SWTL");
    public uint Signature { get; private set; } = BinSignature;

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        var start = reader.At();
        options.Header ??= reader.ReadObject<ChunkHeader>();
        Signature = options.Header.Value.Signature;
        var listOffset = reader.Read<int>();
        Capacity = reader.Read<int>();
        reader.ReadAtOffset((int)start - 8 + listOffset, () => { AddRange(reader.ReadObjectArray<TextureList, ChunkBinaryOptions>(options, Capacity)); });
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.WriteLittle(Signature);
        writer.Write<int>(0); // Size

        var start = writer.At();
        writer.Write(0x10); // List Offset, untracked
        writer.Write(Count);

        writer.WriteObjectCollection(options, this);

        writer.Flush();
        writer.Align(16);
        var end = writer.At();

        var size = (long)end - (long)start;
        writer.At((long)start - sizeof(int), SeekOrigin.Begin);
        writer.Write((int)size);

        end.Dispose();
    }

    public TextureList GetTextureList(string name)
    {
        return Find(item => item.Name == name);
    }
}

public class TextureList : List<Texture>, IBinarySerializable<ChunkBinaryOptions>
{
    public string Name { get; set; }
    public uint Field08 { get; set; }
    public uint Field14 { get; set; }
    public UserData UserData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        if (options.Version >= 3)
            reader.OffsetBinaryFormat = OffsetBinaryFormat.U64;
        
        Name = reader.ReadStringOffset();
        if (options.Version >= 4)
            Field08 = reader.Read<uint>();
        
        Capacity = reader.Read<int>();      
        if (options.Version >= 3)
            reader.Align(8);
        
        AddRange(reader.ReadObjectArrayOffset<Texture, ChunkBinaryOptions>(options, Capacity));
        long userDataOffset = reader.ReadOffsetValue();
        if (userDataOffset != 0)
            UserData = reader.ReadObjectAtOffset<UserData, ChunkBinaryOptions>(userDataOffset, options);
        
        if (options.Version == 0)
            Field14 = reader.Read<uint>();
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
            writer.Write(Field08);
        
        writer.Write(Count);
        writer.WriteObjectCollectionOffset(options, this);
        if (UserData != null)
            writer.WriteObjectOffset(UserData, options);
        else
            writer.WriteOffsetValue(0);
        
        if (options.Version == 0)
            writer.Write(Field14);
    }

    public Texture GetTexture(string name)
    {
        return Find(item => item.Name == name);
    }
}