namespace SharpNeedle.SurfRide.Draw;

public class ReferenceCast : ICast
{
    public Layer Layer { get; set; }
    public uint Field08 { get; set; }
    public uint Field0C { get; set; }
    public uint Field10 { get; set; }
    public uint Field14 { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            reader.Align(8);
        Layer = reader.ReadObjectOffset<Layer, ChunkBinaryOptions>(options);
        Field08 = reader.Read<uint>();
        Field0C = reader.Read<uint>();
        Field10 = reader.Read<uint>();
        Field14 = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            writer.Align(8);
        writer.WriteObjectOffset(Layer, options);
        writer.Write(Field08);
        writer.Write(Field0C);
        writer.Write(Field10);
        writer.Write(Field14);
    }
}