namespace SharpNeedle.Numerics;

// ReSharper disable once InconsistentNaming
public struct AABB : IBinarySerializable
{
    public Vector3 Min;
    public Vector3 Max;

    public float CenterX => (Min.X + Max.X) / 2;
    public float CenterY => (Min.Z + Max.Z) / 2;
    public float CenterZ => (Min.Z + Max.Z) / 2;

    public float SizeX => Max.X - Min.X;
    public float SizeY => Max.Y - Min.Y;
    public float SizeZ => Max.Z - Min.Z;

    public float Size => SizeX * SizeY * SizeZ;

    public void Add(Vector3 point)
    {
        if (point.X < Min.X) Min.X = point.X;
        if (point.Y < Min.Y) Min.Y = point.Y;
        if (point.Z < Min.Z) Min.Z = point.Z;

        if (point.X > Max.X) Max.X = point.X;
        if (point.Y > Max.Y) Max.X = point.Y;
        if (point.Z > Max.Z) Max.X = point.Z;
    }

    public void Read(BinaryObjectReader reader)
    {
        Min.X = reader.Read<float>();
        Max.X = reader.Read<float>();

        Min.Y = reader.Read<float>();
        Max.Y = reader.Read<float>();

        Min.Z = reader.Read<float>();
        Max.Z = reader.Read<float>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(ref Min.X);
        writer.Write(ref Max.X);

        writer.Write(ref Min.Y);
        writer.Write(ref Max.Y);

        writer.Write(ref Min.Z);
        writer.Write(ref Max.Z);
    }

    public override string ToString()
    {
        return $"Max: {Max}, Min: {Min}";
    }
}