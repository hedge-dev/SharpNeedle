namespace SharpNeedle.SurfRide.Draw;

public class UserData : List<Data>, IBinarySerializable<ChunkBinaryOptions>
{
    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        Capacity = reader.Read<int>();
        if (options.Version >= 3)
            reader.Align(8);
        
        AddRange(reader.ReadObjectArrayOffset<Data, ChunkBinaryOptions>(options, Capacity));
    }
    
    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(Count);
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteObjectCollectionOffset(options, this);
    }
}

public class Data : IBinarySerializable<ChunkBinaryOptions>
{
    public string Name { get; set; }
    public int Type { get; set; }
    public Union Value { get; set; }
    public string ValueName { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            reader.Align(8);
        
        Name = reader.ReadStringOffset();
        Type = reader.Read<int>();
        if (options.Version >= 3)
            reader.Align(8);
        
        switch (Type)
        {
            case 0:
                Value = reader.ReadValueOffset<bool>();
                break;
            case 1:
                Value = reader.ReadValueOffset<int>();
                break;
            case 2:
                Value = reader.ReadValueOffset<uint>();
                break;
            case 3:
                Value = reader.ReadValueOffset<float>();
                break;
            case 5:
                ValueName = reader.ReadStringOffset();
                break;
            default:
                Value = reader.ReadValueOffset<uint>();
                break;
        }
        if (options.Version >= 3)
            reader.Align(8);
        else
            reader.Align(4);
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(Type);
        if (options.Version >= 3)
            writer.Align(8);
        
        switch (Type)
        {
            case 0:
                writer.WriteValueOffset(Value.Boolean);
                break;
            case 1:
                writer.WriteValueOffset(Value.Integer);
                break;
            case 2:
                writer.WriteValueOffset(Value.UnsignedInteger);
                break;
            case 3:
                writer.WriteValueOffset(Value.Float);
                break;
            case 5:
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ValueName);
                break;
            default:
                writer.WriteValueOffset(Value.UnsignedInteger);
                break;
        }
        if (options.Version >= 3)
            writer.Align(8);
        else
            writer.Align(4);
    }
}

[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct Union
{
    [FieldOffset(0)] public bool Boolean;
    [FieldOffset(0)] public int Integer;
    [FieldOffset(0)] public uint UnsignedInteger;
    [FieldOffset(0)] public float Float;

    public Union(bool value) : this() => Boolean = value;
    public Union(int value) : this() => Integer = value;
    public Union(uint value) : this() => UnsignedInteger = value;
    public Union(float value) : this() => Float = value;

    public void Set(bool value) => Boolean = value;
    public void Set(int value) => Integer = value;
    public void Set(uint value) => UnsignedInteger = value;
    public void Set(float value) => Float = value;

    public static implicit operator Union(bool value) => new Union(value);
    public static implicit operator Union(int value) => new Union(value);
    public static implicit operator Union(uint value) => new Union(value);
    public static implicit operator Union(float value) => new Union(value);

    public static implicit operator bool(Union value) => value.Boolean;
    public static implicit operator int(Union value) => value.Integer;
    public static implicit operator uint(Union value) => value.UnsignedInteger;
    public static implicit operator float(Union value) => value.Float;
}