namespace SharpNeedle.Framework.Ninja.Csd;

public struct Sprite
{
    public int TextureIndex;
    public Vector2 TopLeft;
    public Vector2 BottomRight;

    public Sprite()
    {
        TopLeft = default;
        TextureIndex = default;
        BottomRight = new(1, 1);
    }
}