namespace SharpNeedle.Framework.SurfRide.Draw;

using SharpNeedle.Structs;

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
        Field00 = reader.ReadVector2();
        Field08 = reader.ReadVector2();
        Field10 = reader.ReadVector2();
        Flags = reader.ReadObject<BitSet<uint>>();
        Color = reader.ReadObject<Color<byte>>();
        Field20 = reader.ReadSingle();
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions context)
    {
        writer.WriteVector2(Field00);
        writer.WriteVector2(Field08);
        writer.WriteVector2(Field10);
        writer.WriteObject(Flags);
        writer.WriteObject(Color);
        writer.WriteSingle(Field20);
    }
}
