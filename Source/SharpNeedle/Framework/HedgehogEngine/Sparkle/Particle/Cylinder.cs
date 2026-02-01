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
        writer.WriteInt32(Enquiangular ? 1 : 0);
        writer.WriteInt32(IsCircumference ? 1 : 0);
        writer.WriteInt32(IsCone ? 1 : 0);
        writer.WriteSingle(Angle);
        writer.WriteSingle(Radius);
        writer.WriteSingle(Height);
        writer.WriteSingle(MinAngle);
        writer.WriteSingle(MaxAngle);
        writer.WriteInt32((int)CylinderEmissionType);
    }
}
