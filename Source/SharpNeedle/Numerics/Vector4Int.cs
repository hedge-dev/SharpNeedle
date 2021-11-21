namespace SharpNeedle.Numerics;

public struct Vector4Int
{
    public int X, Y, Z, W;

    public static implicit operator Vector4(Vector4Int self)
        => new (self.X, self.Y, self.Z, self.W);
}