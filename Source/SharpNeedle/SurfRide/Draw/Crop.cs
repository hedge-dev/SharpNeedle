namespace SharpNeedle.SurfRide.Draw;

public class Crop : IBinarySerializable
{
    public float Left { get; set; }
    public float Top { get; set; }
    public float Right { get; set; }
    public float Bottom { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Left = reader.Read<float>();
        Top = reader.Read<float>();
        Right = reader.Read<float>();
        Bottom = reader.Read<float>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Left);
        writer.Write(Top);
        writer.Write(Right);
        writer.Write(Bottom);
    }
}