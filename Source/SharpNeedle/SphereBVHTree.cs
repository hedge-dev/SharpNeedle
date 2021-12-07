namespace SharpNeedle;

public static class SphereBVHTree
{
    public static BVHNode<Sphere, TValue> Build<TValue>(List<KeyValuePair<Sphere, TValue>> nodes)
    {
        var bNode = new BVHNode<Sphere, TValue>();
        if (nodes.Count == 1)
        {
            bNode.Key = nodes[0].Key;
            bNode.Value = nodes[0].Value;
            return bNode;
        }

        // Avoid stack copies
        var span = CollectionsMarshal.AsSpan(nodes);
        var bounds = new AABB { Center = span[0].Key.Center };
        foreach (var node in span)
            bounds.Add(node.Key);

        bNode.Key = new Sphere(bounds);
        var maxAxis = bounds.Max.MaxAxis();
        var center = bounds.Max.GetAxis(maxAxis);

        var leftNodes = new List<KeyValuePair<Sphere, TValue>>();
        var rightNodes = new List<KeyValuePair<Sphere, TValue>>();
        foreach (var node in span)
        {
            if (node.Key.Center.GetAxis(maxAxis) < center)
                leftNodes.Add(node);
            else
                rightNodes.Add(node);
        }

        if (leftNodes.Count == 0)
            (leftNodes, rightNodes) = (rightNodes, leftNodes);

        if (rightNodes.Count == 0)
        {
            for (int i = 0; i < leftNodes.Count; i++)
            {
                if ((i & 1) == 0)
                    continue;

                rightNodes.Add(leftNodes.Last());
                leftNodes.RemoveAt(leftNodes.Count - 1);
            }
        }

        if (leftNodes.Count != 0)
            bNode.Left = Build(leftNodes);

        if (rightNodes.Count != 0)
            bNode.Right = Build(rightNodes);

        return bNode;
    }
}