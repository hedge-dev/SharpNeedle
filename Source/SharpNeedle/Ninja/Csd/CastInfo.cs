namespace SharpNeedle.Ninja.Csd;

[StructLayout(LayoutKind.Sequential)]
public struct CastInfo
{
    public uint HideFlag;
    public Vector2 Translation;
    public float Rotation;
    public Vector2 Scale;
    public float SpriteIndex;
    public Color<byte> Color;
    public uint GradientTopLeft;
    public uint GradientBottomLeft;
    public uint GradientTopRight;
    public uint GradientBottomRight;
    public uint Field30;
    public uint Field34;
    public uint Field38;
}