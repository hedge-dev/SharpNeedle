
namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public class Effect : IBinarySerializable
{
    public string Name { get; set; }
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
        Name = reader.ReadStringPaddedByte();
        InitialLifeTime = reader.ReadSingle();
        ScaleRatio = reader.ReadSingle();
        GenerateCountRatio = reader.ReadSingle();

        InitialPosition = reader.Read<Vector4>();
        InitialRotation = reader.Read<Vector4>();

        IsLoop = reader.ReadUInt32() == 1;
        DeleteChildren = reader.ReadUInt32() == 1;
        VelocityOffset = reader.ReadSingle();

        FieldU1 = reader.ReadInt32();
        FieldU2 = reader.ReadInt32();
        FieldU3 = reader.ReadInt32();
        FieldU4 = reader.ReadInt32();

        //Footer SEGA
        reader.Seek(16, SeekOrigin.Current);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte(Name);
        writer.Write(InitialLifeTime);
        writer.Write(ScaleRatio);
        writer.Write(GenerateCountRatio);

        writer.Write(InitialPosition.X);
        writer.Write(InitialPosition.Y);
        writer.Write(InitialPosition.Z);
        writer.Write(InitialPosition.W);

        writer.Write(InitialRotation.X);
        writer.Write(InitialRotation.Y);
        writer.Write(InitialRotation.Z);
        writer.Write(InitialRotation.W);

        writer.Write(IsLoop ? 1 : 0);
        writer.Write(DeleteChildren ? 1 : 0);
        writer.Write(VelocityOffset);

        writer.Write(FieldU1);
        writer.Write(FieldU2);
        writer.Write(FieldU3);
        writer.Write(FieldU4);

        //S E G A
        writer.Write(83);
        writer.Write(69);
        writer.Write(71);
        writer.Write(65);
    }
}