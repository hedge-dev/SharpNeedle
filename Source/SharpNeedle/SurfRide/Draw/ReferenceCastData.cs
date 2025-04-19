namespace SharpNeedle.SurfRide.Draw;

public class ReferenceCastData : ICastData
{
    public Layer Layer { get; set; }
    public int Field04 { get; set; }
    public int AnimationID { get; set; }
    public int AnimationFrame { get; set; }
    public int Field10 { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            reader.Align(8);

        Layer = reader.ReadObjectOffset<Layer, ChunkBinaryOptions>(options);
        Field04 = reader.Read<int>();
        AnimationID = reader.Read<int>();
        AnimationFrame = reader.Read<int>();
        Field10 = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteObjectOffset(Layer, options);
        writer.Write(Field04);
        writer.Write(AnimationID);
        writer.Write(AnimationFrame);
        writer.Write(Field10);
    }

    public object Clone()
    {
        return MemberwiseClone();
    }
}