namespace SharpNeedle.SurfRide.Draw;

public class CastCell : ICell
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
    public Vector3 Field20 { get; set; }
    public uint Field30 { get; set; }
    public uint Field34 { get; set; }
    public uint Rotation { get; set; }

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
        Position = reader.Read<Vector3>();
        if (options.Version >= 4)
        {
            reader.Align(16);
            Position = reader.Read<Vector3>();
            reader.Align(16);
            Field20 = reader.Read<Vector3>();
            reader.Align(16);
        }
        else
        {
            Position = reader.Read<Vector3>();
            Field30 = reader.Read<uint>();
            Field34 = reader.Read<uint>();
            Rotation = reader.Read<uint>();
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
            writer.Write(Field20);
            writer.Align(16);
        }
        else
        {
            writer.Write(Position);
            writer.Write(Field30);
            writer.Write(Field34);
            writer.Write(Rotation);
        }
    }
}
