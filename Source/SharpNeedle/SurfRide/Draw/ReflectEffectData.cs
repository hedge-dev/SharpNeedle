namespace SharpNeedle.SurfRide.Draw;

/// <summary>
/// The purpose of this effect is unknown. And it sometimes causes a crash if there are less than 2 or 3 crops.
/// </summary>
public class ReflectEffectData : IEffectData
{
    public EffectType Type => EffectType.Reflect;

    public Vector2 Field00 { get; set; }
    public Vector2 Field08 { get; set; }
    public Vector2 Field10 { get; set; }
    public BitSet<uint> Flags { get; set; }
    public Color<byte> Color { get; set; }
    public float Field20 { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions context)
    {
        Field00 = reader.Read<Vector2>();
        Field08 = reader.Read<Vector2>();
        Field10 = reader.Read<Vector2>();
        Flags = reader.Read<BitSet<uint>>();
        Color = reader.Read<Color<byte>>();
        Field20 = reader.Read<float>();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions context)
    {
        writer.Write(Field00);
        writer.Write(Field08);
        writer.Write(Field10);
        writer.Write(Flags);
        writer.Write(Color);
        writer.Write(Field20);
    }
}
