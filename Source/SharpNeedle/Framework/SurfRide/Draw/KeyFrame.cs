namespace SharpNeedle.Framework.SurfRide.Draw;

using SharpNeedle.Structs;

public class KeyFrame : IBinarySerializable<uint>
{
    public int Frame { get; set; }
    public KeyFrameUnion Value { get; set; }
    public float InParam { get; set; }
    public float OutParam { get; set; }
    public int Field10 { get; set; }

    public void Read(BinaryObjectReader reader, uint flags)
    {
        InterpolationType interpolation = (InterpolationType)(flags & 3);

        Frame = reader.ReadInt32();
        Value = reader.ReadObject<KeyFrameUnion, uint>(flags);
        if (interpolation >= InterpolationType.Hermite)
        {
            InParam = reader.ReadSingle();
            OutParam = reader.ReadSingle();
        }

        if (interpolation == InterpolationType.Individual)
        {
            Field10 = reader.ReadInt32();
        }
    }

    public void Write(BinaryObjectWriter writer, uint flags)
    {
        InterpolationType interpolation = (InterpolationType)(flags & 3);

        writer.WriteInt32(Frame);
        writer.WriteObject(Value, flags);
        if (interpolation >= InterpolationType.Hermite)
        {
            writer.WriteSingle(InParam);
            writer.WriteSingle(OutParam);
        }

        if (interpolation == InterpolationType.Individual)
        {
            writer.WriteInt32(Field10);
        }
    }
}

public enum InterpolationType
{
    Constant,
    Linear,
    Hermite,
    Individual
}

[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct KeyFrameUnion : IBinarySerializable<uint>
{
    [FieldOffset(0)] public float Float;
    [FieldOffset(0)] public int Integer;
    [FieldOffset(0)] public bool Boolean;
    [FieldOffset(0)] public char Character;
    [FieldOffset(0)] public Color<byte> Color;
    [FieldOffset(0)] public uint UnsignedInteger;
    [FieldOffset(0)] public double Double;

    public KeyFrameUnion(float value) : this()
    {
        Float = value;
    }

    public KeyFrameUnion(int value) : this()
    {
        Integer = value;
    }

    public KeyFrameUnion(bool value) : this()
    {
        Boolean = value;
    }

    public KeyFrameUnion(char value) : this()
    {
        Character = value;
    }

    public KeyFrameUnion(Color<byte> value) : this()
    {
        Color = value;
    }

    public KeyFrameUnion(uint value) : this()
    {
        UnsignedInteger = value;
    }

    public KeyFrameUnion(double value) : this()
    {
        Double = value;
    }

    public void Set(float value)
    {
        Float = value;
    }

    public void Set(int value)
    {
        Integer = value;
    }

    public void Set(bool value)
    {
        Boolean = value;
    }

    public void Set(char value)
    {
        Character = value;
    }

    public void Set(Color<byte> value)
    {
        Color = value;
    }

    public void Set(uint value)
    {
        UnsignedInteger = value;
    }

    public void Set(double value)
    {
        Double = value;
    }

    public static implicit operator KeyFrameUnion(float value)
    {
        return new(value);
    }

    public static implicit operator KeyFrameUnion(int value)
    {
        return new(value);
    }

    public static implicit operator KeyFrameUnion(bool value)
    {
        return new(value);
    }

    public static implicit operator KeyFrameUnion(char value)
    {
        return new(value);
    }

    public static implicit operator KeyFrameUnion(Color<byte> value)
    {
        return new(value);
    }

    public static implicit operator KeyFrameUnion(uint value)
    {
        return new(value);
    }

    public static implicit operator KeyFrameUnion(double value)
    {
        return new(value);
    }

    public static implicit operator float(KeyFrameUnion value)
    {
        return value.Float;
    }

    public static implicit operator int(KeyFrameUnion value)
    {
        return value.Integer;
    }

    public static implicit operator bool(KeyFrameUnion value)
    {
        return value.Boolean;
    }

    public static implicit operator char(KeyFrameUnion value)
    {
        return value.Character;
    }

    public static implicit operator Color<byte>(KeyFrameUnion value)
    {
        return value.Color;
    }

    public static implicit operator uint(KeyFrameUnion value)
    {
        return value.UnsignedInteger;
    }

    public static implicit operator double(KeyFrameUnion value)
    {
        return value.Double;
    }

    public void Read(BinaryObjectReader reader, uint flags)
    {
        switch (flags & 0xF0)
        {
            case 0x10:
                Float = reader.ReadSingle();
                break;
            case 0x20:
                Integer = reader.ReadInt32();
                break;
            case 0x30:
                Boolean = reader.Read<bool>();
                break;
            case 0x40:
                Integer = reader.ReadInt32();
                break;
            case 0x50:
                Color = reader.ReadObject<Color<byte>>();
                break;
            case 0x60:
                UnsignedInteger = reader.ReadUInt32();
                break;
            case 0x70:
                Double = reader.ReadDouble();
                break;
            case 0x80:
                Character = reader.Read<char>();
                break;
            default:
                throw new NotImplementedException();
        }

        reader.Align(4);
    }

    public readonly void Write(BinaryObjectWriter writer, uint flags)
    {
        switch (flags & 0xF0)
        {
            case 0x10:
                writer.WriteSingle(Float);
                break;
            case 0x20:
                writer.WriteInt32(Integer);
                break;
            case 0x30:
                writer.Write(Boolean);
                break;
            case 0x40:
                writer.WriteInt32(Integer);
                break;
            case 0x50:
                writer.WriteObject(Color);
                break;
            case 0x60:
                writer.WriteUInt32(UnsignedInteger);
                break;
            case 0x70:
                writer.WriteDouble(Double);
                break;
            case 0x80:
                writer.Write(Character);
                break;
            default:
                throw new NotImplementedException();
        }

        writer.Align(4);
    }
}