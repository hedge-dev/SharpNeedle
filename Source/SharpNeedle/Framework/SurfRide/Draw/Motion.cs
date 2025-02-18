namespace SharpNeedle.Framework.SurfRide.Draw;

public class Motion : IBinarySerializable<ChunkBinaryOptions>
{
    public ushort CastID { get; set; }
    public List<Track> Tracks { get; set; } = [];

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        CastID = reader.Read<ushort>();
        ushort trackCount = reader.Read<ushort>();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Tracks.AddRange(reader.ReadObjectArrayOffset<Track, ChunkBinaryOptions>(options, trackCount));
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(CastID);
        writer.Write((ushort)Tracks.Count);
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        writer.WriteObjectCollectionOffset(options, Tracks);
    }
}