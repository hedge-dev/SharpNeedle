namespace SharpNeedle.Framework.HedgehogEngine.Mirage.MaterialData;
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
        byte valueCount = reader.Read<byte>();
        reader.Skip(1);

        Name = reader.ReadStringOffsetOrEmpty();
        if (valueCount != 0)
        {
            using SeekToken token = reader.ReadOffset();
            for (byte i = 0; i < valueCount; i++)
            {
                // Handle booleans
                if (typeof(T) == typeof(bool))
                {
                    Unsafe.As<List<bool>>(Values)!.Add(reader.Read<int>() != 0);
                }
                else
                {
                    Values!.Add(reader.Read<T>());
                }
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write((byte)(typeof(T) == typeof(bool) ? 0 : 2));
        writer.Write((byte)0);
        writer.Write((byte)Values.Count);
        writer.Write((byte)0);

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        if (typeof(T) != typeof(bool))
        {
            writer.WriteCollectionOffset(Values, 4);
        }
        else
        {
            List<bool> values = Unsafe.As<List<bool>>(Values);
            writer.WriteOffset(() =>
            {
                foreach (bool value in values!)
                {
                    writer.Write(value ? 1 : 0);
                }
            });
        }
    }
}