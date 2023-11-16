namespace SharpNeedle.SurfRide.Draw;

public class Cell : IBinarySerializable<ChunkBinaryOptions>
{
    public Color<byte> MaterialColor { get; set; }
    public Color<byte> IlluminationColor { get; set; }
    public bool IsVisible { get; set; }
    public byte Field09 { get; set; }
    public byte Field0A { get; set; }
    public byte Field0B { get; set; }
    public Vector3 Translation { get; set; }
    public int RotationX { get; set; }
    public int RotationY { get; set; }
    public int RotationZ { get; set; }
    public Vector3 Scale { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        bool is3D = ((Layer)options.Data).Is3D;

        MaterialColor = reader.Read<Color<byte>>();
        IlluminationColor = reader.Read<Color<byte>>();
        IsVisible = reader.Read<bool>();
        Field09 = reader.Read<byte>();
        Field0A = reader.Read<byte>();
        Field0B = reader.Read<byte>();
        if (is3D)
            Read3DTransformation();
        else
            Read2DTransformation();

        void Read3DTransformation()
        {
            if (options.Version >= 4)
            {
                reader.Align(16);
                Translation = reader.Read<Vector3>();
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
                Translation = reader.Read<Vector3>();
                RotationX = reader.Read<int>();
                RotationY = reader.Read<int>();
                RotationZ = reader.Read<int>();
                Scale = reader.Read<Vector3>();
            }
        }

        void Read2DTransformation()
        {
            if (options.Version >= 4)
            {
                reader.Align(16);
                Translation = new(reader.Read<Vector2>(), 0.0f);
                reader.Align(16);
                Scale = new(reader.Read<Vector2>(), 0.0f);
                RotationZ = reader.Read<int>();
                reader.Align(16);
            }
            else
            {
                Translation = new(reader.Read<Vector2>(), 0.0f);
                Scale = new(reader.Read<Vector2>(), 0.0f);
                RotationZ = reader.Read<int>();
            }
        }
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        bool is3D = ((Layer)options.Data).Is3D;

        writer.Write(MaterialColor);
        writer.Write(IlluminationColor);
        writer.Write(IsVisible);
        writer.Write(Field09);
        writer.Write(Field0A);
        writer.Write(Field0B);
        if (is3D)
            Write3DTransformation();
        else
            Write2DTransformation();

        void Write3DTransformation()
        {
            if (options.Version >= 4)
            {
                writer.Align(16);
                writer.Write(Translation);
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
                writer.Write(Translation);
                writer.Write(RotationX);
                writer.Write(RotationY);
                writer.Write(RotationZ);
                writer.Write(Scale);
            }
        }

        void Write2DTransformation()
        {
            if (options.Version >= 4)
            {
                writer.Align(16);
                writer.Write(Translation);
                writer.Align(16);
                writer.Write(Scale);
                writer.Write(RotationZ);
                writer.Align(16);
            }
            else
            {
                writer.Write(Translation);
                writer.Write(Scale);
                writer.Write(RotationZ);
            }
        }
    }
}
