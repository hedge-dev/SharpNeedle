namespace SharpNeedle.Structs;

// ReSharper disable once InconsistentNaming
public struct AABB : IBinarySerializable, IIntersectable<Vector3>, IIntersectable<Sphere>, IIntersectable<AABB>
{
    public Vector3 Min;
    public Vector3 Max;

    public readonly int Length => 8;
    public readonly float CenterX => (Min.X + Max.X) / 2;
    public readonly float CenterY => (Min.Y + Max.Y) / 2;
    public readonly float CenterZ => (Min.Z + Max.Z) / 2;

    public readonly float SizeX => Max.X - Min.X;
    public readonly float SizeY => Max.Y - Min.Y;
    public readonly float SizeZ => Max.Z - Min.Z;

    public float Size => SizeX * SizeY * SizeZ;

    public readonly bool IsEmpty => (Min == Vector3.Zero && Max == Vector3.Zero) || Min == Max;

    public Vector3 Center
    {
        get => new(CenterX, CenterY, CenterZ);
        set
        {
            Vector3 size = new(SizeX / 2, SizeY / 2, SizeZ / 2);
            Min = value - size;
            Max = value + size;
        }
    }

    public float Radius
    {
        get
        {
            float radius = 0f;
            Vector3 center = Center;
            for(int i = 0; i < Length; i++)
            {
                radius = MathF.Max(radius, (center - GetCorner(i)).Length());
            }

            return radius;
        }
    }

    public AABB(Vector3 min, Vector3 max)
    {
        Min = min;
        Max = max;
    }

    public AABB(Sphere bounds)
    {
        Min = bounds.Center - new Vector3(bounds.Radius);
        Max = bounds.Center + new Vector3(bounds.Radius);
    }

    public readonly Vector3 GetCorner(int index)
    {
        return new((index & 1) != 0 ? Max.X : Min.X,
        (index & 2) != 0 ? Max.Y : Min.Y, (index & 4) != 0 ? Max.Z : Min.Z);
    }

    public void Add(Vector3 point)
    {
        if(point.X < Min.X)
        {
            Min.X = point.X;
        }

        if(point.Y < Min.Y)
        {
            Min.Y = point.Y;
        }

        if(point.Z < Min.Z)
        {
            Min.Z = point.Z;
        }

        if(point.X > Max.X)
        {
            Max.X = point.X;
        }

        if(point.Y > Max.Y)
        {
            Max.Y = point.Y;
        }

        if(point.Z > Max.Z)
        {
            Max.Z = point.Z;
        }
    }

    public void Add(AABB other)
    {
        if(other.Min.X < Min.X)
        {
            Min.X = other.Min.X;
        }

        if(other.Min.Y < Min.Y)
        {
            Min.Y = other.Min.Y;
        }

        if(other.Min.Z < Min.Z)
        {
            Min.Z = other.Min.Z;
        }

        if(other.Max.X > Max.X)
        {
            Max.X = other.Max.X;
        }

        if(other.Max.Y > Max.Y)
        {
            Max.Y = other.Max.Y;
        }

        if(other.Max.Z > Max.Z)
        {
            Max.Z = other.Max.Z;
        }
    }

    public void Add(Sphere bounds)
    {
        Add(bounds.Center - new Vector3(bounds.Radius));
        Add(bounds.Center + new Vector3(bounds.Radius));
    }

    public bool Intersects(Vector3 point)
    {
        return !IsEmpty && point.X >= Min.X && point.X <= Max.X &&
                             point.Y >= Min.Y && point.Y <= Max.Y &&
                             point.Z >= Min.Z && point.Z <= Max.Z;
    }

    public bool Intersects(Sphere bounds)
    {
        return !IsEmpty && (Intersects(bounds.Center) ||
                Vector3.DistanceSquared(bounds.Center, Min) <= bounds.Radius * bounds.Radius ||
                Vector3.DistanceSquared(bounds.Center, Max) <= bounds.Radius * bounds.Radius);
    }

    public bool Intersects(AABB aabb)
    {
        return !IsEmpty && aabb.Min.X <= Max.X && aabb.Max.X >= Min.X &&
                aabb.Min.Y <= Max.Y && aabb.Max.Y >= Min.Y &&
                aabb.Min.Z <= Max.Z && aabb.Max.Z >= Min.Z;
    }

    public Vector3 this[int corner] => GetCorner(corner);

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

    public override readonly string ToString()
    {
        return $"Max: {Max}, Min: {Min}";
    }
}