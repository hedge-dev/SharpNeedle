namespace SharpNeedle.SurfRide.Draw.Animations;

public class Motion : IBinarySerializable<ChunkBinaryOptions>
{
    public Animation Animation { get; set; }
    public CastNode Cast { get; set; }
    public ushort CastID { get; set; }
    public List<Track> Tracks { get; set; } = new();

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Animation = (Animation)options.Data;

        CastID = reader.Read<ushort>();

        foreach (var cast in Animation.Layer)
        {
            if (cast.ID != CastID)
                continue;
         
            Cast = cast;
            break;
        }

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