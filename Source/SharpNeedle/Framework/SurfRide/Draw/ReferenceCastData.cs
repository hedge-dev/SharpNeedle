namespace SharpNeedle.Framework.SurfRide.Draw;

public class ReferenceCastData : ICastData
{
    public Layer? Layer { get; set; }
    public int Field04 { get; set; }
    public int AnimationID { get; set; }
    public int AnimationFrame { get; set; }
    public int Field10 { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Layer = reader.ReadObjectOffset<Layer, ChunkBinaryOptions>(options);
        Field04 = reader.ReadInt32();
        AnimationID = reader.ReadInt32();
        AnimationFrame = reader.ReadInt32();
        Field10 = reader.ReadInt32();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (Layer == null)
        {
            throw new InvalidOperationException("Layer is null");
        }

        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        writer.WriteObjectOffset(Layer, options);
        writer.WriteInt32(Field04);
        writer.WriteInt32(AnimationID);
        writer.WriteInt32(AnimationFrame);
        writer.WriteInt32(Field10);
    }
}