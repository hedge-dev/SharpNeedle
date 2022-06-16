namespace SharpNeedle.SurfRide.Draw;

public class Font : List<CharacterMapping>, IBinarySerializable<ChunkBinaryOptions>
{
    public string Name { get; set; }
    public int ID { get; set; }
    public uint Field0C { get; set; }
    public short Field10 { get; set; }
    public ushort Field14 { get; set; }
    public long Field20 { get; set; }
    public UserData UserData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        if (options.Version >= 3)
            reader.Align(8);
        
        Name = reader.ReadStringOffset();
        ID = reader.Read<int>();
        Field0C = reader.Read<uint>();
        if (options.Version >= 4)
            Field10 = reader.Read<short>();
        
        Capacity = reader.Read<ushort>();
        Field14 = reader.Read<ushort>();
        if (options.Version >= 3)
            reader.Align(8);
        else
            reader.Align(4);
        
        AddRange(reader.ReadArrayOffset<CharacterMapping>(Capacity));
        Field20 = reader.ReadOffsetValue();
        long userDataOffset = reader.ReadOffsetValue();
        if (userDataOffset != 0)
            UserData = reader.ReadObjectAtOffset<UserData, ChunkBinaryOptions>(userDataOffset, options);
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(ID);
        writer.Write(Field0C);
        if (options.Version >= 4)
            writer.Write(Field10);
        
        writer.Write((ushort)Count);
        writer.Write(Field14);
        if (options.Version >= 3)
            writer.Align(8);
        else
            writer.Align(4);
        
        writer.WriteCollectionOffset(this);
        writer.WriteOffsetValue(Field20);
        if (UserData != null)
            writer.WriteObjectOffset(UserData, options);
        else
            writer.WriteOffsetValue(0);
    }
}

public struct CharacterMapping
{
    public ushort Character;
    public ushort TextureListIndex;
    public ushort TextureIndex;
    public ushort SpriteIndex;
}