﻿namespace SharpNeedle.Numerics;

[StructLayout(LayoutKind.Sequential)]
public struct Sphere : IIntersectable<Vector3>, IIntersectable<Sphere>
{
    public Vector3 Center;
    public float Radius;

    public Sphere(Vector3 center, float radius)
    {
        Center = center;
        Radius = radius;
    }

    public Sphere(float radius)
    {
        Center = Vector3.Zero;
        Radius = radius;
    }

    public Sphere(AABB volume)
    {
        Center = volume.Center;
        Radius = volume.Radius;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Intersects(Vector3 point)
        => Vector3.DistanceSquared(point, Center) <= (Radius * Radius);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public readonly bool Intersects(Sphere sphere)
        => Vector3.DistanceSquared(Center, sphere.Center) <= (Radius * Radius) + (sphere.Radius * sphere.Radius);
}