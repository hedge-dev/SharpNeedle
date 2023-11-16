namespace SharpNeedle.SurfRide.Draw.Animations;

public class KeyFrame : IBinarySerializable<int>
{
    public int Frame { get; set; }
    public KeyFrameUnion Value { get; set; }
    public float InParam { get; set; }
    public float OutParam { get; set; }
    public int Field10 { get; set; }

    public void Read(BinaryObjectReader reader, int flags)
    {
        var interpolation = (InterpolationType)(flags & 3);

        Frame = reader.Read<int>();
        Value = reader.ReadObject<KeyFrameUnion, int>(flags);
        if (interpolation >= InterpolationType.Hermite)
        {
            InParam = reader.Read<float>();
            OutParam = reader.Read<float>();
        }

        if (interpolation == InterpolationType.Individual)
            Field10 = reader.Read<int>();
    }

    public void Write(BinaryObjectWriter writer, int flags)
    {
        var interpolation = (InterpolationType)(flags & 3);

        writer.Write(Frame);
        writer.WriteObject(Value, flags);
        if (interpolation >= InterpolationType.Hermite)
        {
            writer.Write(InParam);
            writer.Write(OutParam);
        }

        if (interpolation == InterpolationType.Individual)
            writer.Write(Field10);
    }
}

[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct KeyFrameUnion : IBinarySerializable<int>
{
    [FieldOffset(0)] public float Float;
    [FieldOffset(0)] public int Integer;
    [FieldOffset(0)] public bool Boolean;
    [FieldOffset(0)] public char Character;
    [FieldOffset(0)] public Color<byte> Color;
    [FieldOffset(0)] public uint UnsignedInteger;
    [FieldOffset(0)] public double Double;

    public KeyFrameUnion(float value) : this() => Float = value;
    public KeyFrameUnion(int value) : this() => Integer = value;
    public KeyFrameUnion(bool value) : this() => Boolean = value;
    public KeyFrameUnion(char value) : this() => Character = value;
    public KeyFrameUnion(Color<byte> value) : this() => Color = value;
    public KeyFrameUnion(uint value) : this() => UnsignedInteger = value;
    public KeyFrameUnion(double value) : this() => Double = value;

    public void Set(float value) => Float = value;
    public void Set(int value) => Integer = value;
    public void Set(bool value) => Boolean = value;
    public void Set(char value) => Character = value;
    public void Set(Color<byte> value) => Color = value;
    public void Set(uint value) => UnsignedInteger = value;
    public void Set(double value) => Double = value;

    public static implicit operator KeyFrameUnion(float value) => new(value);
    public static implicit operator KeyFrameUnion(int value) => new(value);
    public static implicit operator KeyFrameUnion(bool value) => new(value);
    public static implicit operator KeyFrameUnion(char value) => new(value);
    public static implicit operator KeyFrameUnion(Color<byte> value) => new(value);
    public static implicit operator KeyFrameUnion(uint value) => new(value);
    public static implicit operator KeyFrameUnion(double value) => new(value);

    public static implicit operator float(KeyFrameUnion value) => value.Float;
    public static implicit operator int(KeyFrameUnion value) => value.Integer;
    public static implicit operator bool(KeyFrameUnion value) => value.Boolean;
    public static implicit operator char(KeyFrameUnion value) => value.Character;
    public static implicit operator Color<byte>(KeyFrameUnion value) => value.Color;
    public static implicit operator uint(KeyFrameUnion value) => value.UnsignedInteger;
    public static implicit operator double(KeyFrameUnion value) => value.Double;

    public void Read(BinaryObjectReader reader, int flags)
    {
        switch (flags & 0xF0)
        {
            case 0x10:
                Float = reader.Read<float>();
                break;
            case 0x20:
                Integer = reader.Read<int>();
                break;
            case 0x30:
                Boolean = reader.Read<bool>();
                break;
            case 0x40:
                Integer = reader.Read<int>();
                break;
            case 0x50:
                Color = reader.Read<Color<byte>>();
                break;
            case 0x60:
                UnsignedInteger = reader.Read<uint>();
                break;
            case 0x70:
                Double = reader.Read<double>();
                break;
            case 0x80:
                Character = reader.Read<char>();
                break;
            default:
                throw new NotImplementedException();
        }
        
        reader.Align(4);
    }

    public void Write(BinaryObjectWriter writer, int flags)
    {
        switch (flags & 0xF0)
        {
            case 0x10:
                writer.Write(Float);
                break;
            case 0x20:
                writer.Write(Integer);
                break;
            case 0x30:
                writer.Write(Boolean);
                break;
            case 0x40:
                writer.Write(Integer);
                break;
            case 0x50:
                writer.Write(Color);
                break;
            case 0x60:
                writer.Write(UnsignedInteger);
                break;
            case 0x70:
                writer.Write(Double);
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