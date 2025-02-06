namespace SharpNeedle.Framework.Ninja.Csd;

using SharpNeedle.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct CastInfo : IBinarySerializable
{
    public uint HideFlag;
    public Vector2 Translation;
    public float Rotation;
    public Vector2 Scale;
    public float SpriteIndex;
    public Color<byte> Color;
    public Color<byte> GradientTopLeft;
    public Color<byte> GradientBottomLeft;
    public Color<byte> GradientTopRight;
    public Color<byte> GradientBottomRight;
    public uint Field30;
    public uint Field34;
    public uint Field38;

    public void Read(BinaryObjectReader reader)
    {
        HideFlag = reader.Read<uint>();
        Translation = reader.Read<Vector2>();
        Rotation = reader.Read<float>();
        Scale = reader.Read<Vector2>();
        SpriteIndex = reader.Read<float>();

        // Colours are handled as uints for endianness
        Unsafe.As<Color<byte>, uint>(ref Color) = reader.Read<uint>();
        Unsafe.As<Color<byte>, uint>(ref GradientTopLeft) = reader.Read<uint>();
        Unsafe.As<Color<byte>, uint>(ref GradientBottomLeft) = reader.Read<uint>();
        Unsafe.As<Color<byte>, uint>(ref GradientTopRight) = reader.Read<uint>();
        Unsafe.As<Color<byte>, uint>(ref GradientBottomRight) = reader.Read<uint>();
        
        Field30 = reader.Read<uint>();
        Field34 = reader.Read<uint>();
        Field38 = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(HideFlag);
        writer.Write(Translation);
        writer.Write(Rotation);
        writer.Write(Scale);
        writer.Write(SpriteIndex);

        // Colours are handled as uints for endianness
        writer.Write(Unsafe.As<Color<byte>, uint>(ref Color));
        writer.Write(Unsafe.As<Color<byte>, uint>(ref GradientTopLeft));
        writer.Write(Unsafe.As<Color<byte>, uint>(ref GradientBottomLeft));
        writer.Write(Unsafe.As<Color<byte>, uint>(ref GradientTopRight));
        writer.Write(Unsafe.As<Color<byte>, uint>(ref GradientBottomRight));

        writer.Write(Field30);
        writer.Write(Field34);
        writer.Write(Field38);
    }
}