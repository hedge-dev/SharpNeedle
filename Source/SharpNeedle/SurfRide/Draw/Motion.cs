namespace SharpNeedle.SurfRide.Draw;

public class Motion : IBinarySerializable<ChunkBinaryOptions>
{
    public ushort CastID { get; set; }
    public List<Track> Tracks { get; set; } = new();

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        CastID = reader.Read<ushort>();
        var trackCount = reader.Read<ushort>();
        if (options.Version >= 3)
            reader.Align(8);
        
        Tracks.AddRange(reader.ReadObjectArrayOffset<Track, ChunkBinaryOptions>(options, trackCount));
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(CastID);
        writer.Write((ushort)Tracks.Count);
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteObjectCollectionOffset(options, Tracks);
    }
}

public enum FCurveType
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