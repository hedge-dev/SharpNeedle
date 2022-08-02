namespace SharpNeedle.SurfRide.Draw;

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
        Field00 = reader.Read<int>();
        Field04 = reader.Read<int>();
        CropCount = reader.Read<int>();
        Step = reader.Read<int>();
        Blend = reader.Read<BlendMode>();
        Color = reader.Read<Color<byte>>();
        Field18 = reader.Read<int>();
        Field1C = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions context)
    {
        writer.Write(Field00);
        writer.Write(Field04);
        writer.Write(CropCount);
        writer.Write(Step);
        writer.Write(Blend);
        writer.Write(Color);
        writer.Write(Field18);
        writer.Write(Field1C);
    }

    public enum BlendMode
    {
        Default, Add, Mode2, Mode3
    }
}