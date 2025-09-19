
namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public class Effect : IBinarySerializable
{
    public string EffectName;
    public float InitialLifeTime;
    public float ScaleRatio;
    public float GenerateCountRatio;
    public Vector4 InitialPosition;
    public Vector4 InitialRotation;
    public bool IsLoop;
    public bool DeleteChildren;
    public float VelocityOffset;
    public int FieldU1, FieldU2, FieldU3, FieldU4;
   
    public void Read(BinaryObjectReader reader)
    {        
        EffectName = reader.ReadStringPaddedByte();
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
        writer.WriteStringPaddedByte(EffectName);
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