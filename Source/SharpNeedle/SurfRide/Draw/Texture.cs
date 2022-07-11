namespace SharpNeedle.SurfRide.Draw;

public class Texture : List<Rectangle>, IBinarySerializable<ChunkBinaryOptions>
{
    public string Name { get; set; }
    public string FileName { get; set; }
    public int ID { get; set; }
    public ushort Width { get; set; }
    public ushort Height { get; set; }
    public uint Flags { get; set; }
    public UserData UserData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        if (options.Version >= 3)
            reader.Align(8);

        Name = reader.ReadStringOffset();
        if (options.Version >= 5)
            FileName = reader.ReadStringOffset();
        
        ID = reader.Read<int>();
        Width = reader.Read<ushort>();
        Height = reader.Read<ushort>();
        Flags = reader.Read<uint>();
        Capacity = reader.Read<int>();
        if (Capacity != 0)
            AddRange(reader.ReadObjectArrayOffset<Rectangle>(Capacity));
        else
            reader.ReadOffsetValue();
        
        long userDataOffset = reader.ReadOffsetValue();
        if (userDataOffset != 0)
            UserData = reader.ReadObjectAtOffset<UserData, ChunkBinaryOptions>(userDataOffset, options);
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            writer.Align(8);

        if (options.Version < 5 && FileName != null)
            Name = FileName;
        
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        if (options.Version >= 5)
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, FileName);

        writer.Write(ID);
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(Flags);
        writer.Write(Count);
        if (Count != 0)
            writer.WriteObjectCollectionOffset(this);
        else
            writer.WriteOffsetValue(0);
        
        if (UserData != null)
            writer.WriteObjectOffset(UserData, options);
        else
            writer.WriteOffsetValue(0);
    }
}
public class Rectangle : IBinarySerializable
{
    public float Left { get; set; }
    public float Top { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Left = reader.Read<float>();
        Top = reader.Read<float>();
        Right = reader.Read<float>();
        Bottom = reader.Read<float>();
    }
    
    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Left);
        writer.Write(Top);
        writer.Write(Right);
        writer.Write(Bottom);
    }
}