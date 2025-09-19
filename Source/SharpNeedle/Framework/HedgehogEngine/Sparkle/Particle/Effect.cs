
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
    public int ukn0, ukn1, ukn2, ukn3;
   
    public void Read(BinaryObjectReader reader)
    {        
        EffectName = reader.ReadStringPaddedByte();
        InitialLifeTime = reader.ReadSingle();
        ScaleRatio = reader.ReadSingle();
        GenerateCountRatio = reader.ReadSingle();

        InitialPosition = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        InitialRotation = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());

        IsLoop = reader.ReadUInt32() == 1;
        DeleteChildren = reader.ReadUInt32() == 1;
        VelocityOffset = reader.ReadSingle();

        ukn0 = reader.ReadInt32();
        ukn1 = reader.ReadInt32();
        ukn2 = reader.ReadInt32();
        ukn3 = reader.ReadInt32();

        //Footer SEGA
        reader.Seek(4 * 4, SeekOrigin.Current);
        //for (int i = 0; i < 4; i++)
        //{
        //    Console.Write(System.Text.Encoding.UTF8.GetString(reader.ReadArray<byte>(1)));
        //    reader.Seek(3, SeekOrigin.Current);
        //}
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

        writer.Write(ukn0);
        writer.Write(ukn1);
        writer.Write(ukn2);
        writer.Write(ukn3);

        //S E G A
        writer.Write(83);
        writer.Write(69);
        writer.Write(71);
        writer.Write(65);
    }
}