namespace SharpNeedle.Numerics;

public struct Vector4Int
{
    public int X, Y, Z, W;

    public Vector4Int(int x, int y, int z, int w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
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