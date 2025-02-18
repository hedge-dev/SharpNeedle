namespace SharpNeedle.Utilities;

using SharpNeedle.Structs;

public class BVHNode<TKey, TValue>
{
    public BVHNodeType Type => Left == null && Right == null ? BVHNodeType.Leaf : BVHNodeType.Branch;
    public TKey Key { get; set; }
    public BVHNode<TKey, TValue>? Left { get; set; }
    public BVHNode<TKey, TValue>? Right { get; set; }
    public TValue? Value { get; set; }

    public BVHNode(TKey key)
    {
        Key = key;
        Value = default;
    }

    public BVHNode(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}

public static class BVHNode
{
    public static bool Traverse<TKey, TValue, TPoint>(this BVHNode<TKey, TValue> node, TPoint point, Action<BVHNode<TKey, TValue>> callback) where TKey : IIntersectable<TPoint>
    {
        if (node == null || !node.Key.Intersects(point))
        {
            return false;
        }

        if (node.Type == BVHNodeType.Leaf)
        {
            callback(node);
            return true;
        }

        bool hasLeft = node.Left?.Traverse(point, callback) == true;
        bool hasRight = node.Right?.Traverse(point, callback) == true;
        return hasLeft || hasRight;
    }

    public static bool Traverse<TPoint, TKey, TValue>(this BVHNode<TKey, TValue> node, TPoint point, Action<BVHNode<TKey, TValue>> callback) where TPoint : IIntersectable<TKey>
    {
        if (node == null || !point.Intersects(node.Key))
        {
            return false;
        }

        if (node.Type == BVHNodeType.Leaf)
        {
            callback(node);
            return true;
        }

        bool hasLeft = node.Left?.Traverse(point, callback) == true;
        bool hasRight = node.Right?.Traverse(point, callback) == true;
        return hasLeft || hasRight;
    }
}

public enum BVHNodeType
{
    Branch,
    Leaf
}