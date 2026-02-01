namespace SharpNeedle.Structs;

public struct Vector4Int : IBinarySerializable
{
    public int X, Y, Z, W;

    public Vector4Int(int x, int y, int z, int w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public void Read(BinaryObjectReader reader)
    {
        X = reader.ReadInt32();
        Y = reader.ReadInt32();
        Z = reader.ReadInt32();
        W = reader.ReadInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
        writer.WriteInt32(Z);
        writer.WriteInt32(W);
    }

    public static Vector4Int operator +(Vector4Int a, Vector4Int b)
    {
        return new(a.X + b.X, a.Y + b.Y, a.Z + b.Z, a.W + b.W);
    }

    public static Vector4Int operator -(Vector4Int a, Vector4Int b)
    {
        return new(a.X - b.X, a.Y - b.Y, a.Z - b.Z, a.W - b.W);
    }

    public static Vector4Int operator *(Vector4Int a, Vector4Int b)
    {
        return new(a.X * b.X, a.Y * b.Y, a.Z * b.Z, a.W * b.W);
    }

    public static Vector4Int operator /(Vector4Int a, Vector4Int b)
    {
        return new(a.X / b.X, a.Y / b.Y, a.Z / b.Z, a.W / b.W);
    }

    public static implicit operator Vector4(Vector4Int self)
    {
        return new(self.X, self.Y, self.Z, self.W);
    }
}