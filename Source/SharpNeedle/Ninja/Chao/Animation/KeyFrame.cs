namespace SharpNeedle.Ninja.Chao.Animation;

[StructLayout(LayoutKind.Sequential)]
public struct KeyFrame
{
    public uint Frame;
    public float Value;
    public uint Field08;
    public float Offset1;
    public float Offset2;
    public uint Field14;
}