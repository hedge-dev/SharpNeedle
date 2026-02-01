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
        CurveType = (FCurveType)reader.ReadInt16();
        Capacity = reader.ReadUInt16();
        Flags = reader.ReadUInt32();
        StartFrame = reader.ReadUInt32();
        EndFrame = reader.ReadUInt32();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        AddRange(reader.ReadObjectArrayOffset<KeyFrame, uint>(Flags, Capacity));
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.WriteInt16((short)CurveType);
        writer.WriteUInt16((ushort)Count);
        writer.WriteUInt32(Flags);
        writer.WriteUInt32(StartFrame);
        writer.WriteUInt32(EndFrame);
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