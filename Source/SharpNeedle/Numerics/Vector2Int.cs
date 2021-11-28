namespace SharpNeedle.Numerics;

public struct Vector2Int
{
    public int X, Y;

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
        => new (a.X + b.X, a.Y + b.Y);

    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
        => new (a.X - b.X, a.Y - b.Y);

    public static Vector2Int operator *(Vector2Int a, Vector2Int b)
        => new (a.X * b.X, a.Y * b.Y);

    public static Vector2Int operator /(Vector2Int a, Vector2Int b)
        => new (a.X / b.X, a.Y / b.Y);

    public static implicit operator Vector2(Vector2Int self)
        => new(self.X, self.Y);
}