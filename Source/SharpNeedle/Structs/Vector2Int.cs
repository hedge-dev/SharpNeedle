namespace SharpNeedle.Structs;

public struct Vector2Int : IBinarySerializable
{
    public int X, Y;

    public Vector2Int(int x, int y)
    {
        X = x;
        Y = y;
    }

    public void Read(BinaryObjectReader reader)
    {
        X = reader.ReadInt32();
        Y = reader.ReadInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(X);
        writer.WriteInt32(Y);
    }

    public static Vector2Int operator +(Vector2Int a, Vector2Int b)
    {
        return new(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2Int operator -(Vector2Int a, Vector2Int b)
    {
        return new(a.X - b.X, a.Y - b.Y);
    }

    public static Vector2Int operator *(Vector2Int a, Vector2Int b)
    {
        return new(a.X * b.X, a.Y * b.Y);
    }

    public static Vector2Int operator /(Vector2Int a, Vector2Int b)
    {
        return new(a.X / b.X, a.Y / b.Y);
    }

    public static implicit operator Vector2(Vector2Int self)
    {
        return new(self.X, self.Y);
    }
}