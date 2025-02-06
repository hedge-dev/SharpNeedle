namespace SharpNeedle.Utilities;

using SharpNeedle.Structs;

public static class SphereBVHTree
{
    public static BVHNode<Sphere, TValue> Build<TValue>(List<KeyValuePair<Sphere, TValue>> nodes)
    {
        if (nodes.Count == 1)
        {
            return new(nodes[0].Key, nodes[0].Value);
        }

        // Avoid stack copies
        Span<KeyValuePair<Sphere, TValue>> span = CollectionsMarshal.AsSpan(nodes);
        AABB bounds = new()
        { Center = span[0].Key.Center };
        foreach (KeyValuePair<Sphere, TValue> node in span)
        {
            bounds.Add(node.Key);
        }

        BVHNode<Sphere, TValue> bNode = new(new(bounds));

        int maxAxis = bounds.Max.MaxAxis();
        float center = bounds.Max.GetAxis(maxAxis);

        List<KeyValuePair<Sphere, TValue>> leftNodes = [];
        List<KeyValuePair<Sphere, TValue>> rightNodes = [];
        foreach (KeyValuePair<Sphere, TValue> node in span)
        {
            if (node.Key.Center.GetAxis(maxAxis) < center)
            {
                leftNodes.Add(node);
            }
            else
            {
                rightNodes.Add(node);
            }
        }

        if (leftNodes.Count == 0)
        {
            (leftNodes, rightNodes) = (rightNodes, leftNodes);
        }

        if (rightNodes.Count == 0)
        {
            for (int i = 0; i < leftNodes.Count; i++)
            {
                if ((i & 1) == 0)
                {
                    continue;
                }

                rightNodes.Add(leftNodes.Last());
                leftNodes.RemoveAt(leftNodes.Count - 1);
            }
        }

        if (leftNodes.Count != 0)
        {
            bNode.Left = Build(leftNodes);
        }

        if (rightNodes.Count != 0)
        {
            bNode.Right = Build(rightNodes);
        }

        return bNode;
    }
}