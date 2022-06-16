namespace SharpNeedle.SurfRide.Draw;
using Animation;

public class Layer : IBinarySerializable<ChunkBinaryOptions>
{
    public string Name { get; set; }
    public int ID { get; set; }
    public uint Flags { get; set; }
    public uint DefaultAnimationIndex { get; set; }
    public List<CastNode> CastNodes { get; set; } = new();
    public List<ICell> CastCells { get; set; } = new();
    public List<AnimationData> Animations { get; set; } = new();
    public UserData UserData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            reader.Align(8);
        
        Name = reader.ReadStringOffset();
        ID = reader.Read<int>();
        Flags = reader.Read<uint>();
        var castCount = reader.Read<int>();
        if (options.Version >= 3)
            reader.Align(8);
        
        CastNodes.AddRange(reader.ReadObjectArrayOffset<CastNode, ChunkBinaryOptions>(options, castCount));
        switch (Flags & 0xF)
        {
            case 0:
                CastCells.AddRange(reader.ReadObjectArrayOffset<CastCell, ChunkBinaryOptions>(options, castCount));
                break;
            case 1:
                CastCells.AddRange(reader.ReadObjectArrayOffset<CastCell3D, ChunkBinaryOptions>(options, castCount));
                break;
            default:
                throw new NotImplementedException();
        }
        var animCount = reader.Read<int>();
        if (options.Version >= 3)
            reader.Align(8);

        Animations.AddRange(reader.ReadObjectArrayOffset<AnimationData, ChunkBinaryOptions>(options, animCount));
        DefaultAnimationIndex = reader.Read<uint>();
        if (options.Version >= 3)
            reader.Align(8);
        
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
        writer.Write(Flags);
        writer.Write(CastNodes.Count);
        if (options.Version >= 3)
            writer.Align(8);
        
        if (CastNodes.Count != 0)
        {
            writer.WriteObjectCollectionOffset(options, CastNodes);
            if (options.Version >= 4)
            {
                writer.WriteOffset(() =>
                {
                    foreach (var cell in CastCells)
                        writer.WriteObject(cell, options);
                }, 16);
            }
            else
            {
                writer.WriteObjectCollectionOffset(options, CastCells);
            }
        }
        else
        {
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
        }
        writer.Write(Animations.Count);
        if (options.Version >= 3)
            writer.Align(8);
        
        if (Animations.Count != 0)
            writer.WriteObjectCollectionOffset(options, Animations);
        else
            writer.WriteOffsetValue(0);
        
        writer.Write(DefaultAnimationIndex);
        if (options.Version >= 3)
            writer.Align(8);

        if (UserData != null)
            writer.WriteObjectOffset(UserData, options);
        else
            writer.WriteOffsetValue(0);
    }
}
