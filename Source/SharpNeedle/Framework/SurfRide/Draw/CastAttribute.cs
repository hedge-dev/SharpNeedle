namespace SharpNeedle.Framework.SurfRide.Draw;

[Flags]
public enum CastAttribute
{
    BlendModeDefault = 0,
    BlendModeAdd = 1,
    BlendModeSubtract = 2,
    BlendModeMultiply = 4,
    FlipHorizontal = 0x10,
    FlipVertical = 0x20,
    RotateLeft = 0x40,
    FlipHorizontalAndVertical = 0x80,
    UseFont = 0x100,
    AntiAliasing = 0x2000,
    PivotPositionBottomRight = 0x40000,
    PivotPositionBottomCenter = 0x80000,
    PivotPositionBottomLeft = 0x100000,
    PivotPositionMiddleRight = 0x180000,
    PivotPositionMiddleCenter = 0x200000,
    PivotPositionMiddleLeft = 0x280000,
    PivotPositionTopRight = 0x300000,
    PivotPositionTopCenter = 0x380000,
    PivotPositionTopLeft = 0x400000
}