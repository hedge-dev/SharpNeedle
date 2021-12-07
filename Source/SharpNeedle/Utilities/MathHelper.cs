namespace SharpNeedle.Utilities;

public static class MathHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MaxAxis(this Vector2 vector)
        => vector.X > vector.Y ? 0 : 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MaxAxis(this Vector3 vector)
        => vector.X > vector.Y && vector.X > vector.Z ? 0 : vector.Y > vector.Z ? 1 : 2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MaxAxis(this Vector4 vector)
        => vector.X > vector.Y && vector.X > vector.Z && vector.X > vector.W ? 0 :
            vector.Y > vector.Z && vector.Y > vector.W ? 1 :
            vector.Z > vector.W ? 2 : 3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MinAxis(this Vector2 vector)
        => vector.X < vector.Y ? 0 : 1;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MinAxis(this Vector3 vector)
        => vector.X < vector.Y && vector.X < vector.Z ? 0 : vector.Y < vector.Z ? 1 : 2;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int MinAxis(this Vector4 vector)
        => vector.X < vector.Y && vector.X < vector.Z && vector.X < vector.W ? 0 :
            vector.Y < vector.Z && vector.Y < vector.W ? 1 :
            vector.Z < vector.W ? 2 : 3;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(this Vector2 vector)
        => MathF.Max(vector.X, vector.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(this Vector3 vector)
        => MathF.Max(vector.X, MathF.Max(vector.Y, vector.Z));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Max(this Vector4 vector)
        => MathF.Max(vector.X, MathF.Max(vector.Y, MathF.Min(vector.Z, vector.W)));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(this Vector2 vector)
        => MathF.Min(vector.X, vector.Y);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(this Vector3 vector)
        => MathF.Min(vector.X, MathF.Min(vector.Y, vector.Z));

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float Min(this Vector4 vector)
        => MathF.Min(vector.X, MathF.Min(vector.Y, MathF.Min(vector.Z, vector.W)));

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe ref float GetAxis(this Vector2 vector, int idx)
    {
        if (idx is < 0 or >= 2)
            throw new IndexOutOfRangeException($"{idx} is < 0 or >= 2");

        return ref ((float*)&vector)[idx];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe ref float GetAxis(this Vector3 vector, int idx)
    {
        if (idx is < 0 or >= 3)
            throw new IndexOutOfRangeException($"{idx} is < 0 or >= 3");

        return ref ((float*)&vector)[idx];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static unsafe ref float GetAxis(this Vector4 vector, int idx)
    {
        if (idx is < 0 or >= 4)
            throw new IndexOutOfRangeException($"{idx} is < 0 or >= 4");

        return ref ((float*)&vector)[idx];
    }
}