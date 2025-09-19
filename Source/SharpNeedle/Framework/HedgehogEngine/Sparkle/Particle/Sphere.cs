namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public class Sphere : IBinarySerializable
{
    public float m_latitude_max_angle;
    public float m_longitude_max_angle;

    public void Read(BinaryObjectReader reader)
    {
        m_latitude_max_angle = reader.ReadSingle();
        m_longitude_max_angle = reader.ReadSingle();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(m_latitude_max_angle);
        writer.Write(m_longitude_max_angle);
    }
}
