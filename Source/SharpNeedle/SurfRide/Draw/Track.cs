namespace SharpNeedle.SurfRide.Draw;

public class Track : List<KeyFrame>, IBinarySerializable<ChunkBinaryOptions>, ICloneable
{
    public FCurveType CurveType { get; set; }
    public int Flags { get; set; }
    public uint StartFrame { get; set; }
    public uint EndFrame { get; set; }

    public Track()
    {

    }

    public Track(Track track)
    {
        CurveType = track.CurveType;
        Flags = track.Flags;
        StartFrame = track.StartFrame;
        EndFrame = track.EndFrame;
        foreach (var keyframe in track)
        {
            Add(new(keyframe));
        }
    }

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

    public object Clone()
    {
        Track clone = MemberwiseClone() as Track;
        for (int i = 0; i < Count; i++)
            clone[i] = this[i].Clone() as KeyFrame;

        return clone;
    }
}

public enum FCurveType : short
{
    Tx,
    Ty,
    Tz,
    Rx,
    Ry,
    Rz,
    Sx,
    Sy,
    Sz,
    MaterialColor,
    Display,
    Width,
    Height,
    VertexColorTopLeft,
    VertexColorTopRight,
    VertexColorBottomLeft,
    VertexColorBottomRight,
    CropIndex0,
    CropIndex1,
    IlluminationColor = 20,
    MaterialColorR,
    MaterialColorG,
    MaterialColorB,
    MaterialColorA,
    VertexColorTopLeftR,
    VertexColorTopLeftG,
    VertexColorTopLeftB,
    VertexColorTopLeftA,
    VertexColorTopRightR,
    VertexColorTopRightG,
    VertexColorTopRightB,
    VertexColorTopRightA,
    VertexColorBottomLeftR,
    VertexColorBottomLeftG,
    VertexColorBottomLeftB,
    VertexColorBottomLeftA,
    VertexColorBottomRightR,
    VertexColorBottomRightG,
    VertexColorBottomRightB,
    VertexColorBottomRightA,
    IlluminationColorR,
    IlluminationColorG,
    IlluminationColorB,
    IlluminationColorA
}