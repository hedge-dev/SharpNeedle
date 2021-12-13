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

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static Vector4 GetColumn(this Matrix4x4 matrix, int idx)
    {
        switch (idx)
        {
            case 0:
                return new Vector4(matrix.M11, matrix.M21, matrix.M31, matrix.M41);

            case 1:
                return new Vector4(matrix.M12, matrix.M22, matrix.M32, matrix.M42);

            case 2:
                return new Vector4(matrix.M13, matrix.M23, matrix.M33, matrix.M43);

            case 3:
                return new Vector4(matrix.M14, matrix.M24, matrix.M34, matrix.M44);
        }

        throw new IndexOutOfRangeException();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    public static void SetColumn(ref this Matrix4x4 matrix, int idx, Vector4 value)
    {
        switch (idx)
        {
            case 0:
                matrix.M11 = value.X;
                matrix.M21 = value.Y;
                matrix.M31 = value.Z;
                matrix.M41 = value.W;
                return;

            case 1:
                matrix.M12 = value.X;
                matrix.M22 = value.Y;
                matrix.M32 = value.Z;
                matrix.M42 = value.W;
                return;

            case 2:
                matrix.M13 = value.X;
                matrix.M23 = value.Y;
                matrix.M33 = value.Z;
                matrix.M43 = value.W;
                return;

            case 3:
                matrix.M14 = value.X;
                matrix.M24 = value.Y;
                matrix.M34 = value.Z;
                matrix.M44 = value.W;
                return;
        }

        throw new IndexOutOfRangeException($"{idx} is < 0 or >= 3");
    }

    public static unsafe ref Vector4 GetRow(ref this Matrix4x4 matrix, int idx)
    {
        if (idx is < 0 or >= 4)
            throw new IndexOutOfRangeException($"{idx} is < 0 or >= 4");

        fixed (Matrix4x4* pMatrix = &matrix)
        {
            return ref ((Vector4*)pMatrix)[idx];
        }
    }

    public static Vector3 ToEuler(this Quaternion quaternion)
    {
        var sqw = quaternion.W * quaternion.W;
        var sqx = quaternion.X * quaternion.X;
        var sqy = quaternion.Y * quaternion.Y;
        var sqz = quaternion.Z * quaternion.Z;

        var unit = sqx + sqy + sqz + sqw;
        var test = quaternion.X * quaternion.Y + quaternion.Z * quaternion.W;

        if (test > 0.499f * unit)
        {
            return new Vector3(2.0f * MathF.Atan2(quaternion.X, quaternion.W), MathF.PI / 2.0f, 0.0f);
        }

        if (test < -0.499f * unit)
        {
            return new Vector3(-2.0f * MathF.Atan2(quaternion.X, quaternion.W), -MathF.PI / 2.0f, 0.0f);
        }

        return new Vector3(
            MathF.Atan2(2.0f * (quaternion.Y * quaternion.W - quaternion.X * quaternion.Z), sqx - sqy - sqz + sqw),
            MathF.Asin(2.0f * test / unit),
            MathF.Atan2(2.0f * (quaternion.X * quaternion.W + quaternion.Y * quaternion.Z), -sqx + sqy - sqz + sqw));
    }
}