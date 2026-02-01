namespace SharpNeedle.Framework.SurfRide.Draw;

public class Font : List<CharData>, IBinarySerializable<ChunkBinaryOptions>
{
    public string? Name { get; set; }
    public int ID { get; set; }
    public uint Field0C { get; set; }
    public short Field10 { get; set; }
    public ushort Field14 { get; set; }
    public long Field20 { get; set; }
    public UserData? UserData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Name = reader.ReadStringOffset();
        ID = reader.ReadInt32();
        Field0C = reader.ReadUInt32();
        if (options.Version >= 4)
        {
            Field10 = reader.ReadInt16();
        }

        Capacity = reader.ReadUInt16();
        Field14 = reader.ReadUInt16();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }
        else
        {
            reader.Align(4);
        }

        AddRange(reader.ReadObjectArrayOffset<CharData>(Capacity));
        Field20 = reader.ReadOffsetValue();
        long userDataOffset = reader.ReadOffsetValue();
        if (userDataOffset != 0)
        {
            UserData = reader.ReadObjectAtOffset<UserData, ChunkBinaryOptions>(userDataOffset, options);
        }
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.WriteInt32(ID);
        writer.WriteUInt32(Field0C);
        if (options.Version >= 4)
        {
            writer.WriteInt16(Field10);
        }

        writer.WriteUInt16((ushort)Count);
        writer.WriteUInt16(Field14);
        if (options.Version >= 3)
        {
            writer.Align(8);
        }
        else
        {
            writer.Align(4);
        }

        writer.WriteObjectCollectionOffset(this);
        writer.WriteOffsetValue(Field20);
        if (UserData != null)
        {
            writer.WriteObjectOffset(UserData, options);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }
    }
}

public class CharData : IBinarySerializable
{
    public ushort Code;
    public ushort TextureListIndex;
    public ushort TextureIndex;
    public ushort CropIndex;

    public void Read(BinaryObjectReader reader)
    {
        Code = reader.ReadUInt16();
        TextureListIndex = reader.ReadUInt16();
        TextureIndex = reader.ReadUInt16();
        CropIndex = reader.ReadUInt16();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteUInt16(Code);
        writer.WriteUInt16(TextureListIndex);
        writer.WriteUInt16(TextureIndex);
        writer.WriteUInt16(CropIndex);
    }
}