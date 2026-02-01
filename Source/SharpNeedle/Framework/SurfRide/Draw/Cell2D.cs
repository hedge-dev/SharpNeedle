namespace SharpNeedle.Framework.SurfRide.Draw;

using SharpNeedle.Structs;

public class Cell2D : ICell
{
    public Color<byte> MaterialColor { get; set; }
    public Color<byte> IlluminationColor { get; set; }
    public byte Field08 { get; set; }
    public byte Field09 { get; set; }
    public byte Field0A { get; set; }
    public byte Field0B { get; set; }
    public Vector2 Translation { get; set; }
    public Vector2 Scale { get; set; }
    public uint RotationZ { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        MaterialColor = reader.ReadObject<Color<byte>>();
        IlluminationColor = reader.ReadObject<Color<byte>>();
        Field08 = reader.ReadByte();
        Field09 = reader.ReadByte();
        Field0A = reader.ReadByte();
        Field0B = reader.ReadByte();
        Translation = reader.ReadVector2();
        if (options.Version >= 4)
        {
            reader.Align(16);
            Translation = reader.ReadVector2();
            reader.Align(16);
            Scale = reader.ReadVector2();
            RotationZ = reader.ReadUInt32();
            reader.Align(16);
        }
        else
        {
            Translation = reader.ReadVector2();
            Scale = reader.ReadVector2();
            RotationZ = reader.ReadUInt32();
        }
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.WriteObject(MaterialColor);
        writer.WriteObject(IlluminationColor);
        writer.WriteByte(Field08);
        writer.WriteByte(Field09);
        writer.WriteByte(Field0A);
        writer.WriteByte(Field0B);
        if (options.Version >= 4)
        {
            writer.Align(16);
            writer.WriteVector2(Translation);
            writer.Align(16);
            writer.WriteVector2(Scale);
            writer.WriteUInt32(RotationZ);
            writer.Align(16);
        }
        else
        {
            writer.WriteVector2(Translation);
            writer.WriteVector2(Scale);
            writer.WriteUInt32(RotationZ);
        }
    }
}
