namespace SharpNeedle.Framework.SurfRide.Draw;

public class Texture : List<Crop>, IBinarySerializable<ChunkBinaryOptions>
{
    public string? Name { get; set; }
    public string? TextureFileName { get; set; }
    public int ID { get; set; }
    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public uint Flags { get; set; }
    public UserData? UserData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Name = reader.ReadStringOffset();
        if (options.Version >= 5)
        {
            TextureFileName = reader.ReadStringOffset();
        }

        ID = reader.ReadInt32();
        Width = reader.ReadUInt16();
        Height = reader.ReadUInt16();
        Flags = reader.ReadUInt32();
        Capacity = reader.ReadInt32();
        if (Capacity != 0)
        {
            AddRange(reader.ReadObjectArrayOffset<Crop>(Capacity));
        }
        else
        {
            reader.ReadOffsetValue();
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

        if (options.Version < 5 && TextureFileName != null)
        {
            Name = TextureFileName;
        }

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        if (options.Version >= 5)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, TextureFileName);
        }

        writer.WriteInt32(ID);
        writer.WriteUInt16(Width);
        writer.WriteUInt16(Height);
        writer.WriteUInt32(Flags);
        writer.WriteInt32(Count);
        if (Count != 0)
        {
            writer.WriteObjectCollectionOffset(this);
        }
        else
        {
            writer.WriteOffsetValue(0);
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