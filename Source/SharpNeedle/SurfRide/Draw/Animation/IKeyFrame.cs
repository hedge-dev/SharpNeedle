namespace SharpNeedle.SurfRide.Draw.Animation;

public interface IKeyFrame : IBinarySerializable<uint>
{
    public int Frame { get; set; }
    public Union Value { get; set; }
}

[StructLayout(LayoutKind.Explicit, Size = 4)]
public struct Union
{
    [FieldOffset(0)] public float Float;
    [FieldOffset(0)] public int Integer;
    [FieldOffset(0)] public bool Boolean;
    [FieldOffset(0)] public char Character;
    [FieldOffset(0)] public Color<byte> Color;
    [FieldOffset(0)] public uint UnsignedInteger;
    [FieldOffset(0)] public double Double;

    public Union(float value) : this() => Float = value;
    public Union(int value) : this() => Integer = value;
    public Union(bool value) : this() => Boolean = value;
    public Union(char value) : this() => Character = value;
    public Union(Color<byte> value) : this() => Color = value;
    public Union(uint value) : this() => UnsignedInteger = value;
    public Union(double value) : this() => Double = value;

    public void Set(float value) => Float = value;
    public void Set(int value) => Integer = value;
    public void Set(bool value) => Boolean = value;
    public void Set(char value) => Character = value;
    public void Set(Color<byte> value) => Color = value;
    public void Set(uint value) => UnsignedInteger = value;
    public void Set(double value) => Double = value;

    public static implicit operator Union(float value) => new Union(value);
    public static implicit operator Union(int value) => new Union(value);
    public static implicit operator Union(bool value) => new Union(value);
    public static implicit operator Union(char value) => new Union(value);
    public static implicit operator Union(Color<byte> value) => new Union(value);
    public static implicit operator Union(uint value) => new Union(value);
    public static implicit operator Union(double value) => new Union(value);

    public static implicit operator float(Union value) => value.Float;
    public static implicit operator int(Union value) => value.Integer;
    public static implicit operator bool(Union value) => value.Boolean;
    public static implicit operator char(Union value) => value.Character;
    public static implicit operator Color<byte>(Union value) => value.Color;
    public static implicit operator uint(Union value) => value.UnsignedInteger;
    public static implicit operator double(Union value) => value.Double;
}