namespace SharpNeedle.Framework.SurfRide.Draw;

using SharpNeedle.Structs;

public class BlurEffectData : IEffectData
{
    public EffectType Type => EffectType.Blur;

    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int CropCount { get; set; }
    public int Step { get; set; }
    public BlendMode Blend { get; set; }
    public Color<byte> Color { get; set; }
    public int Field18 { get; set; }
    public int Field1C { get; set; }


    public void Read(BinaryObjectReader reader, ChunkBinaryOptions context)
    {
        Field00 = reader.ReadInt32();
        Field04 = reader.ReadInt32();
        CropCount = reader.ReadInt32();
        Step = reader.ReadInt32();
        Blend = (BlendMode)reader.ReadInt32();
        Color = reader.ReadObject<Color<byte>>();
        Field18 = reader.ReadInt32();
        Field1C = reader.ReadInt32();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions context)
    {
        writer.WriteInt32(Field00);
        writer.WriteInt32(Field04);
        writer.WriteInt32(CropCount);
        writer.WriteInt32(Step);
        writer.WriteInt32((int)Blend);
        writer.WriteObject(Color);
        writer.WriteInt32(Field18);
        writer.WriteInt32(Field1C);
    }

    public enum BlendMode
    {
        Default, Add, Mode2, Mode3
    }
}