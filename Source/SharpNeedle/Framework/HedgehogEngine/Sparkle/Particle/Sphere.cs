namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public class Sphere : IBinarySerializable
{
    public float LatitudeMaxAngle { get; set; }
    public float LongitudeMaxAngle { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        LatitudeMaxAngle = reader.ReadSingle();
        LongitudeMaxAngle = reader.ReadSingle();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(LatitudeMaxAngle);
        writer.Write(LongitudeMaxAngle);
    }
}
