namespace SharpNeedle.Utilities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public static class StructBinaryHelper
{
    // use these to avoid AOT endian byte swapping issues

    public static Vector2 ReadVector2(this BinaryValueReader reader)
    {
        return new Vector2(
            reader.ReadSingle(),
            reader.ReadSingle()
        );
    }

    public static void WriteVector2(this BinaryValueWriter writer, Vector2 value)
    {
        writer.WriteSingle(value.X);
        writer.WriteSingle(value.Y);
    }

    public static Vector2[] ReadVector2Array(this BinaryValueReader reader, int count)
    {
        Vector2[] result = new Vector2[count];

        for (int i = 0; i < count; i++)
        {
            result[i] = reader.ReadVector2();
        }

        return result;
    }

    public static void WriteVector2Array(this BinaryValueWriter writer, Vector2[] array)
    {
        foreach (Vector2 value in array)
        {
            writer.WriteVector2(value);
        }
    }

    public static Vector3 ReadVector3(this BinaryValueReader reader)
    {
        return new Vector3(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle()
        );
    }

    public static void WriteVector3(this BinaryValueWriter writer, Vector3 value)
    {
        writer.WriteSingle(value.X);
        writer.WriteSingle(value.Y);
        writer.WriteSingle(value.Z);
    }

    public static Vector3[] ReadVector3Array(this BinaryValueReader reader, int count)
    {
        Vector3[] result = new Vector3[count];

        for(int i = 0; i < count; i++)
        {
            result[i] = reader.ReadVector3();
        }

        return result;
    }

    public static void WriteVector3Array(this BinaryValueWriter writer, Vector3[] array)
    {
        foreach (Vector3 value in array)
        {
            writer.WriteVector3(value);
        }
    }

    public static Vector4 ReadVector4(this BinaryValueReader reader)
    {
        return new Vector4(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle()
        );
    }

    public static void WriteVector4(this BinaryValueWriter writer, Vector4 value)
    {
        writer.WriteSingle(value.X);
        writer.WriteSingle(value.Y);
        writer.WriteSingle(value.Z);
        writer.WriteSingle(value.W);
    }

    public static Quaternion ReadQuaternion(this BinaryValueReader reader)
    {
        return new Quaternion(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle()
        );
    }

    public static void WriteQuaternion(this BinaryValueWriter writer, Quaternion value)
    {
        writer.WriteSingle(value.X);
        writer.WriteSingle(value.Y);
        writer.WriteSingle(value.Z);
        writer.WriteSingle(value.W);
    }

    public static Matrix4x4 ReadMatrix4x4(this BinaryValueReader reader)
    {
        return new Matrix4x4(
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle(),
            reader.ReadSingle()
        );
    }

    public static void WriteMatrix4x4(this BinaryValueWriter writer, Matrix4x4 value)
    {
        writer.WriteSingle(value.M11);
        writer.WriteSingle(value.M12);
        writer.WriteSingle(value.M13);
        writer.WriteSingle(value.M14);
        writer.WriteSingle(value.M21);
        writer.WriteSingle(value.M22);
        writer.WriteSingle(value.M23);
        writer.WriteSingle(value.M24);
        writer.WriteSingle(value.M31);
        writer.WriteSingle(value.M32);
        writer.WriteSingle(value.M33);
        writer.WriteSingle(value.M34);
        writer.WriteSingle(value.M41);
        writer.WriteSingle(value.M42);
        writer.WriteSingle(value.M43);
        writer.WriteSingle(value.M44);
    }

    public static Guid ReadGuid(this BinaryValueReader reader)
    {
        return new Guid(reader.ReadArray<byte>(16), reader.Endianness == Endianness.Big);
    }

    public static void WriteGuid(this BinaryValueWriter writer, Guid guid)
    {
        writer.WriteBytes(guid.ToByteArray(writer.Endianness == Endianness.Big));
    }
}
