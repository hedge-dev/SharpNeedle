namespace SharpNeedle.SurfRide.Draw;

public class Motion : IBinarySerializable<ChunkBinaryOptions>, ICloneable
{
    public ushort CastID { get; set; }
    public CastNode Cast { get; set; }
    public Animation Animation { get; set; }
    public List<Track> Tracks { get; set; } = new();

    public Motion()
    {

    }

    public Motion(Motion motion)
    {
        CastID = motion.CastID;
        Cast = motion.Cast;
        Animation = motion.Animation;
        foreach (var track in motion.Tracks)
            Tracks.Add(new(track));
    }

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

    public object Clone()
    {
        Motion clone = MemberwiseClone() as Motion;
        for (int i = 0; i < Tracks.Count; i++)
            clone.Tracks[i] = Tracks[i].Clone() as Track;

        return clone;
    }
}