namespace SharpNeedle.Framework.SurfRide.Draw;

public class CastNode : IBinarySerializable<ChunkBinaryOptions>
{
    public string? Name { get; set; }
    public int ID { get; set; }
    public uint Flags { get; set; }
    public short ChildIndex { get; set; }
    public short SiblingIndex { get; set; }
    public ICastData? Data { get; set; }
    public UserData? UserData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Name = reader.ReadStringOffset();
        ID = reader.ReadInt32();
        Flags = reader.ReadUInt32();
        switch (Flags & 0xF)
        {
            case 1:
                Data = reader.ReadObjectOffset<ImageCastData, ChunkBinaryOptions>(options);
                break;
            case 2:
                Data = reader.ReadObjectOffset<SliceCastData, ChunkBinaryOptions>(options);
                break;
            case 3:
                Data = reader.ReadObjectOffset<ReferenceCastData, ChunkBinaryOptions>(options);
                break;
            default:
                reader.ReadOffsetValue(); // 0 == NullCast
                break;
        }

        ChildIndex = reader.ReadInt16();
        SiblingIndex = reader.ReadInt16();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

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
        writer.WriteUInt32(Flags);
        if (Data != null)
        {
            writer.WriteObjectOffset(Data, options);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        writer.WriteInt16(ChildIndex);
        writer.WriteInt16(SiblingIndex);
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

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