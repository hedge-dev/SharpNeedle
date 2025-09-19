namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;
public class Cylinder : IBinarySerializable
{
    public bool Enquiangular { get; set; }
    public bool IsCircumference { get; set; }
    public bool IsCone { get; set; }
    public float Angle { get; set; }
    public float Radius { get; set; }
    public float Height { get; set; }
    public float MinAngle { get; set; }
    public float MaxAngle { get; set; }
    public CylinderEmissionType CylinderEmissionType { get; set; }

    public void Read(BinaryObjectReader reader)
    {        
        Enquiangular = reader.ReadUInt32() == 1;
        IsCircumference = reader.ReadUInt32() == 1;
        IsCone = reader.ReadUInt32() == 1;
        Angle = reader.ReadSingle();
        Radius = reader.ReadSingle();
        Height = reader.ReadSingle();
        MinAngle = reader.ReadSingle();
        MaxAngle = reader.ReadSingle();
        CylinderEmissionType = (CylinderEmissionType)reader.ReadInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(Enquiangular ? 1 : 0);
        writer.Write(IsCircumference ? 1 : 0);
        writer.Write(IsCone ? 1 : 0);
        writer.Write(Angle);
        writer.Write(Radius);
        writer.Write(Height);
        writer.Write(MinAngle);
        writer.Write(MaxAngle);
        writer.Write(CylinderEmissionType);
    }
}
