namespace SharpNeedle.Framework.SurfRide.Draw;

public class Crop : IBinarySerializable
{
    public float Left { get; set; }
    public float Top { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Left = reader.ReadSingle();
        Top = reader.ReadSingle();
        Right = reader.ReadSingle();
        Bottom = reader.ReadSingle();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteSingle(Left);
        writer.WriteSingle(Top);
        writer.WriteSingle(Right);
        writer.WriteSingle(Bottom);
    }
}