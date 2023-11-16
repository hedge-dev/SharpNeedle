namespace SharpNeedle.SurfRide.Draw.Animations;

public class Track : List<KeyFrame>, IBinarySerializable<ChunkBinaryOptions>
{
    public FCurveType CurveType { get; set; }
    public int Flags { get; set; }
    public uint StartFrame { get; set; }
    public uint EndFrame { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        CurveType = reader.Read<FCurveType>();
        Capacity = reader.Read<ushort>();
        Flags = reader.Read<int>();
        StartFrame = reader.Read<uint>();
        EndFrame = reader.Read<uint>();
        if (options.Version >= 3)
            reader.Align(8);
        
        AddRange(reader.ReadObjectArrayOffset<KeyFrame, int>(Flags, Capacity));
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(CurveType);
        writer.Write((ushort)Count);
        writer.Write(Flags);
        writer.Write(StartFrame);
        writer.Write(EndFrame);
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteObjectCollectionOffset(Flags, this);
    }
}