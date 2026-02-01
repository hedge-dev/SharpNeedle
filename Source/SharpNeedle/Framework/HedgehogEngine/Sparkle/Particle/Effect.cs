
namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public class Effect : IBinarySerializable
{
    public string? Name { get; set; }
    public float InitialLifeTime { get; set; }
    public float ScaleRatio { get; set; }
    public float GenerateCountRatio { get; set; }
    public Vector4 InitialPosition { get; set; }
    public Vector4 InitialRotation { get; set; }
    public bool IsLoop { get; set; }
    public bool DeleteChildren { get; set; }
    public float VelocityOffset { get; set; }
    public int FieldU1 { get; set; }
    public int FieldU2 { get; set; }
    public int FieldU3 { get; set; }
    public int FieldU4 { get; set; }

    public void Read(BinaryObjectReader reader)
    {        
        Name = reader.ReadStringPaddedByte(4);
        InitialLifeTime = reader.ReadSingle();
        ScaleRatio = reader.ReadSingle();
        GenerateCountRatio = reader.ReadSingle();

        InitialPosition = reader.ReadVector4();
        InitialRotation = reader.ReadVector4();

        IsLoop = reader.ReadUInt32() == 1;
        DeleteChildren = reader.ReadUInt32() == 1;
        VelocityOffset = reader.ReadSingle();

        FieldU1 = reader.ReadInt32();
        FieldU2 = reader.ReadInt32();
        FieldU3 = reader.ReadInt32();
        FieldU4 = reader.ReadInt32();

        //Footer SEGA
        reader.Skip(16);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte(Name, 4);
        writer.WriteSingle(InitialLifeTime);
        writer.WriteSingle(ScaleRatio);
        writer.WriteSingle(GenerateCountRatio);

        writer.WriteVector4(InitialPosition);
        writer.WriteVector4(InitialRotation);

        writer.WriteInt32(IsLoop ? 1 : 0);
        writer.WriteInt32(DeleteChildren ? 1 : 0);
        writer.WriteSingle(VelocityOffset);

        writer.WriteInt32(FieldU1);
        writer.WriteInt32(FieldU2);
        writer.WriteInt32(FieldU3);
        writer.WriteInt32(FieldU4);

        //S E G A
        writer.WriteInt32(83);
        writer.WriteInt32(69);
        writer.WriteInt32(71);
        writer.WriteInt32(65);
    }
}