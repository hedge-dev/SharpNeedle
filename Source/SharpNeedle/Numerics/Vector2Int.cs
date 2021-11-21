namespace SharpNeedle.Numerics;

public struct Vector2Int
{
    public int X, Y;

    public static implicit operator Vector2(Vector2Int self)
        => new(self.X, self.Y);
}