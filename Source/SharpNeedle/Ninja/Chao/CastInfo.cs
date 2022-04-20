namespace SharpNeedle.Ninja.Chao;

[StructLayout(LayoutKind.Sequential)]
public struct CastInfo
{
    public int Field00;
    public Vector2 Translation;
    public float Rotation;
    public Vector2 Scale;
    public int Field18;
    public uint Color;
    public uint GradientTopLeft;
    public uint GradientBottomLeft;
    public uint GradientTopRight;
    public uint GradientBottomRight;
    public uint Field30;
    public uint Field34;
    public uint Field38;
}