namespace SharpNeedle.Framework.LostWorld.Animation;

public struct PlayModeInfo
{
    public PlayMode Mode;
    public short RandomMin;
    public short RandomMax;
}

public enum PlayMode
{
    Loop, Stop
}