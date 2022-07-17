namespace SharpNeedle.SurfRide.Draw;

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
        MaterialColor = reader.Read<Color<byte>>();
        IlluminationColor = reader.Read<Color<byte>>();
        Field08 = reader.Read<byte>();
        Field09 = reader.Read<byte>();
        Field0A = reader.Read<byte>();
        Field0B = reader.Read<byte>();
        if (options.Version >= 4)
        {
            reader.Align(16);
            Position = reader.Read<Vector3>();
            reader.Align(16);
            RotationX = reader.Read<int>();
            RotationY = reader.Read<int>();
            RotationZ = reader.Read<int>();
            reader.Align(16);
            Scale = reader.Read<Vector3>();
            reader.Align(16);
        }
        else
        {
            Position = reader.Read<Vector3>();
            RotationX = reader.Read<int>();
            RotationY = reader.Read<int>();
            RotationZ = reader.Read<int>();
            Scale = reader.Read<Vector3>();
        }
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(MaterialColor);
        writer.Write(IlluminationColor);
        writer.Write(Field08);
        writer.Write(Field09);
        writer.Write(Field0A);
        writer.Write(Field0B);
        if (options.Version >= 4)
        {
            writer.Align(16);
            writer.Write(Position);
            writer.Align(16);
            writer.Write(RotationX);
            writer.Write(RotationY);
            writer.Write(RotationZ);
            writer.Align(16);
            writer.Write(Scale);
            writer.Align(16);
        }
        else
        {
            writer.Write(Position);
            writer.Write(RotationX);
            writer.Write(RotationY);
            writer.Write(RotationZ);
            writer.Write(Scale);
        }
    }
}
