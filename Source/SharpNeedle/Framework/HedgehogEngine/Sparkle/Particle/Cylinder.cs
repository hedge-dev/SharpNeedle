namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;
public class Cylinder : IBinarySerializable
{
    public bool m_equiangularly;
    public bool m_circumference;
    public bool m_isCone;
    public float m_angle;
    public float m_radius;
    public float m_height;
    public float m_minAngle;
    public float m_maxAngle;
    public int m_cylinderEmittionType;

    public void Read(BinaryObjectReader reader)
    {        
        m_equiangularly = reader.ReadUInt32() == 1;
        m_circumference = reader.ReadUInt32() == 1;
        m_isCone = reader.ReadUInt32() == 1;
        m_angle = reader.ReadSingle();
        m_radius = reader.ReadSingle();
        m_height = reader.ReadSingle();
        m_minAngle = reader.ReadSingle();
        m_maxAngle = reader.ReadSingle();
        m_cylinderEmittionType = reader.ReadInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(m_equiangularly ? 1 : 0);
        writer.Write(m_circumference ? 1 : 0);
        writer.Write(m_isCone ? 1 : 0);
        writer.Write(m_angle);
        writer.Write(m_radius);
        writer.Write(m_height);
        writer.Write(m_minAngle);
        writer.Write(m_maxAngle);
        writer.Write(m_cylinderEmittionType);
    }
}
