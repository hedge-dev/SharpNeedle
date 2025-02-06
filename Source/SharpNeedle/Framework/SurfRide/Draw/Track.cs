namespace SharpNeedle.Framework.SurfRide.Draw;

public class Track : List<KeyFrame>, IBinarySerializable<ChunkBinaryOptions>
{
    public FCurveType CurveType { get; set; }
    public uint Flags { get; set; }
    public uint StartFrame { get; set; }
    public uint EndFrame { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        CurveType = reader.Read<FCurveType>();
        Capacity = reader.Read<ushort>();
        Flags = reader.Read<uint>();
        StartFrame = reader.Read<uint>();
        EndFrame = reader.Read<uint>();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        AddRange(reader.ReadObjectArrayOffset<KeyFrame, uint>(Flags, Capacity));
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(CurveType);
        writer.Write((ushort)Count);
        writer.Write(Flags);
        writer.Write(StartFrame);
        writer.Write(EndFrame);
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        writer.WriteObjectCollectionOffset(Flags, this);
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
    Unknown1,
    Unknown2,
    Unknown3,
    Unknown4,
    Unknown5,
    Unknown6,
    Unknown7,
    Unknown8,
    Unknown9,
    IlluminationColor,
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