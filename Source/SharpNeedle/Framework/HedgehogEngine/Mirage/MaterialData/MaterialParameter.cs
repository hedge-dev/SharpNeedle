namespace SharpNeedle.Framework.HedgehogEngine.Mirage.MaterialData;

using SharpNeedle.Structs;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

public class MaterialParameter<T> : IBinarySerializable where T : unmanaged
{
    internal string Name { get; set; } = string.Empty;

    public List<T> Values { get; set; } = new(1);

    [JsonIgnore]
    public T Value
    {
        get
        {
            if (Values.Count == 0 || Values == null)
            {
                return default;
            }

            return Values[0];
        }
        set
        {
            Values ??= new List<T>(1);

            if (Values.Count == 0)
            {
                Values.Add(value);
            }
            else
            {
                Values[0] = value;
            }
        }
    }

    public void Read(BinaryObjectReader reader)
    {
        reader.Skip(2);
        byte valueCount = reader.ReadByte();
        reader.Skip(1);

        Name = reader.ReadStringOffsetOrEmpty();
        if (valueCount != 0)
        {
            using SeekToken token = reader.ReadOffset();
            switch (Values)
            {
                case List<bool> booleanValues:
                    for (int i = 0; i < valueCount; i++)
                    {
                        booleanValues.Add(reader.ReadInt32() != 0);
                    }
                    break;
                case List<Vector4> vector4Values:
                    for (int i = 0; i < valueCount; i++)
                    {
                        vector4Values.Add(reader.ReadVector4());
                    }
                    break;
                case List<Vector4Int> vector4IntValues:
                    for (int i = 0; i < valueCount; i++)
                    {
                        vector4IntValues.Add(reader.ReadObject<Vector4Int>());
                    }
                    break;

                default:
                    throw new NotSupportedException($"Type {typeof(T)} is not a valid material parameter type!");
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteByte((byte)(typeof(T) == typeof(bool) ? 0 : 2));
        writer.WriteByte(0);
        writer.WriteByte((byte)Values.Count);
        writer.WriteByte(0);

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.WriteOffset(() =>
        {
            switch (Values)
            {
                case List<bool> booleanValues:
                    foreach (bool value in booleanValues)
                    {
                        writer.WriteInt32(value ? 1 : 0);
                    }
                    break;
                case List<Vector4> vector4Values:
                    foreach (Vector4 value in vector4Values)
                    {
                        writer.WriteVector4(value);
                    }
                    break;
                case List<Vector4Int> vector4IntValues:
                    foreach (Vector4Int value in vector4IntValues)
                    {
                        writer.WriteObject(value);
                    }
                    break;
                default:
                    throw new NotSupportedException($"Type {typeof(T)} is not a valid material parameter type!");
            }

        });
    }
}