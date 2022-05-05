namespace SharpNeedle.Ninja.Cell;

public struct Image
{
    public int TextureIndex;
    public Vector2 TopLeft;
    public Vector2 BottomRight = new(1, 1);
}