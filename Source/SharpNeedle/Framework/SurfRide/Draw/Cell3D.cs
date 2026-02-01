namespace SharpNeedle.Framework.SurfRide.Draw;

using SharpNeedle.Structs;

public class Cell3D : ICell
{
    public Color<byte> MaterialColor { get; set; }
    public Color<byte> IlluminationColor { get; set; }
    public byte Field08 { get; set; }
    public byte Field09 { get; set; }
    public byte Field0A { get; set; }
    public byte Field0B { get; set; }
    public Vector3 Position { get; set; }
    public int RotationX { get; set; }
    public int RotationY { get; set; }
    public int RotationZ { get; set; }
    public Vector3 Scale { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        MaterialColor = reader.ReadObject<Color<byte>>();
        IlluminationColor = reader.ReadObject<Color<byte>>();
        Field08 = reader.ReadByte();
        Field09 = reader.ReadByte();
        Field0A = reader.ReadByte();
        Field0B = reader.ReadByte();
        if (options.Version >= 4)
        {
            reader.Align(16);
            Position = reader.ReadVector3();
            reader.Align(16);
            RotationX = reader.ReadInt32();
            RotationY = reader.ReadInt32();
            RotationZ = reader.ReadInt32();
            reader.Align(16);
            Scale = reader.ReadVector3();
            reader.Align(16);
        }
        else
        {
            Position = reader.ReadVector3();
            RotationX = reader.ReadInt32();
            RotationY = reader.ReadInt32();
            RotationZ = reader.ReadInt32();
            Scale = reader.ReadVector3();
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
            writer.WriteVector3(Position);
            writer.Align(16);
            writer.WriteInt32(RotationX);
            writer.WriteInt32(RotationY);
            writer.WriteInt32(RotationZ);
            writer.Align(16);
            writer.WriteVector3(Scale);
            writer.Align(16);
        }
        else
        {
            writer.WriteVector3(Position);
            writer.WriteInt32(RotationX);
            writer.WriteInt32(RotationY);
            writer.WriteInt32(RotationZ);
            writer.WriteVector3(Scale);
        }
    }
}
