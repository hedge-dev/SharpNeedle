namespace SharpNeedle.SurfRide.Draw;

public class ReferenceCast : ICastData
{
    public Layer Layer { get; set; }
    public long Field08 { get; set; }
    public long Field0C { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            reader.Align(8);

        Layer = reader.ReadObjectOffset<Layer, ChunkBinaryOptions>(options);
        Field08 = reader.ReadOffsetValue();
        Field0C = reader.ReadOffsetValue();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteObjectOffset(Layer, options);
        writer.WriteOffsetValue(Field08);
        writer.WriteOffsetValue(Field0C);
    }
}