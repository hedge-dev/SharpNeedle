namespace SharpNeedle.SurfRide.Draw;

public class CastCell3D : ICell
{
    public uint Color { get; set; }
    public byte Field04 { get; set; }
    public byte Field05 { get; set; }
    public byte Field06 { get; set; }
    public byte Field07 { get; set; }
    public byte Field08 { get; set; }
    public byte Field09 { get; set; }
    public byte Field0A { get; set; }
    public byte Field0B { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Color = reader.Read<uint>();
        Field04 = reader.Read<byte>();
        Field05 = reader.Read<byte>();
        Field06 = reader.Read<byte>();
        Field07 = reader.Read<byte>();
        Field08 = reader.Read<byte>();
        Field09 = reader.Read<byte>();
        Field0A = reader.Read<byte>();
        Field0B = reader.Read<byte>();
        if (options.Version >= 4)
        {
            reader.Align(16);
            Position = reader.Read<Vector3>();
            reader.Align(16);
            Rotation = reader.Read<Vector3>();
            reader.Align(16);
            Scale = reader.Read<Vector3>();
            reader.Align(16);
        }
        else
        {
            Position = reader.Read<Vector3>();
            Rotation = reader.Read<Vector3>();
            Scale = reader.Read<Vector3>();
        }
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(Color);
        writer.Write(Field04);
        writer.Write(Field05);
        writer.Write(Field06);
        writer.Write(Field07);
        writer.Write(Field08);
        writer.Write(Field09);
        writer.Write(Field0A);
        writer.Write(Field0B);
        if (options.Version >= 4)
        {
            writer.Align(16);
            writer.Write(Position);
            writer.Align(16);
            writer.Write(Rotation);
            writer.Align(16);
            writer.Write(Scale);
            writer.Align(16);
        }
        else
        {
            writer.Write(Position);
            writer.Write(Rotation);
            writer.Write(Scale);
        }
    }
}
