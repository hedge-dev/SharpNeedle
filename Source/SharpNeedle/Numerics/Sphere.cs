namespace SharpNeedle.Numerics;

public struct Sphere
{
    public Vector3 Position;
    public float Radius;

    public Sphere(Vector3 position, float radius)
    {
        Position = position;
        Radius = radius;
    }

    public Sphere(float radius)
    {
        Position = Vector3.Zero;
        Radius = radius;
    }

    public bool Intersects(Vector3 point)
        => Vector3.DistanceSquared(point, Position) <= (Radius * Radius);

    public bool Intersects(Sphere sphere)
        => Vector3.DistanceSquared(Position, sphere.Position) <= (Radius * Radius) + (sphere.Radius * sphere.Radius);
}