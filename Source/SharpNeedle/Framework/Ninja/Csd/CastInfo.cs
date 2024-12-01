namespace SharpNeedle.Framework.Ninja.Csd;

using SharpNeedle.Structs;

[StructLayout(LayoutKind.Sequential)]
public struct CastInfo
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
}