namespace SharpNeedle.Framework.HedgehogEngine.Mirage.TerrainData;
using SharpNeedle.Framework.HedgehogEngine.Mirage;

using SharpNeedle.Structs;

[NeedleResource("hh/tbst", @"\.tbst$")]
public class TerrainBlockSphereTree : SampleChunkResource
{
    public new BVHNode<Sphere, (int GroupID, int SubsetID)> Root { get; set; } = new(default);

    public static TerrainBlockSphereTree Build(List<TerrainGroup> groups)
    {
        List<KeyValuePair<Sphere, (int GroupID, int SubsetID)>> nodes = new(groups.Capacity);

        for (int groupId = 0; groupId < groups.Count; groupId++)
        {
            TerrainGroup group = groups[groupId];
            for (int subsetID = 0; subsetID < group.Subsets.Count; subsetID++)
            {
                TerrainGroupSubset set = group.Subsets[subsetID];
                nodes.Add(new(set.Bounds, (groupId, subsetID)));
            }
        }

        TerrainBlockSphereTree tree = new()
        { Root = SphereBVHTree.Build(nodes) };
        return tree;
    }

    public bool Traverse(Vector3 point, Action<BVHNode<Sphere, (int GroupID, int SubsetID)>> callback)
    {
        return Root.Traverse(point, callback);
    }

    public bool Traverse(Sphere bounds, Action<BVHNode<Sphere, (int GroupID, int SubsetID)>> callback)
    {
        return Root.Traverse<Sphere, (int, int), Sphere>(bounds, callback);
    }

    public bool Traverse<TPoint>(TPoint point, Action<BVHNode<Sphere, (int GroupID, int SubsetID)>> callback) where TPoint : IIntersectable<Sphere>
    {
        return Root.Traverse(point, callback);
    }

    public override void Read(BinaryObjectReader reader)
    {
        int nodeCount = reader.ReadInt32();
        BinaryNode[] nodes = new BinaryNode[nodeCount];
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < nodeCount; i++)
            {
                nodes[i] = reader.ReadObjectOffset<BinaryNode>();
            }
        });

        int rootIndex = reader.ReadInt32();
        Root = BuildNode(ref nodes[rootIndex]);

        BVHNode<Sphere, (int, int)> BuildNode(ref BinaryNode node)
        {
            BVHNode<Sphere, (int GroupID, int SubsetID)> result = new(node.Bounds);

            if (node.Type == NodeType.Branch)
            {
                result.Left = BuildNode(ref nodes[node.LeftIndex]);
                result.Right = BuildNode(ref nodes[node.RightIndex]);
            }
            else
            {
                result.Value = new(node.TerrainGroupIndex, node.GroupSubsetIndex);
            }

            return result;
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        List<BinaryNode> nodes = [];
        BuildNodes(Root, nodes);

        writer.WriteInt32(nodes.Count);
        writer.WriteOffset(() =>
        {
            foreach (BinaryNode node in nodes)
            {
                writer.WriteObjectOffset(node);
            }
        });
        writer.WriteInt32(nodes.Count - 1);

        static void BuildNodes(BVHNode<Sphere, (int, int)> node, List<BinaryNode> result)
        {
            BinaryNode bNode = new()
            {
                Bounds = node.Key,
                Type = (NodeType)node.Type
            };

            if (node.Type == BVHNodeType.Leaf)
            {
                bNode.TerrainGroupIndex = node.Value.Item1;
                bNode.GroupSubsetIndex = node.Value.Item2;
            }
            else
            {
                if (node.Left != null)
                {
                    bNode.LeftIndex = result.Count;
                    BuildNodes(node.Left, result);
                }

                if (node.Right != null)
                {
                    bNode.RightIndex = result.Count;
                    BuildNodes(node.Right, result);
                }
            }

            result.Add(bNode);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct BinaryNode : IBinarySerializable
    {
        [FieldOffset(0)] public NodeType Type;
        [FieldOffset(4)] public int LeftIndex;
        [FieldOffset(8)] public int RightIndex;

        [FieldOffset(4)] public int TerrainGroupIndex;
        [FieldOffset(8)] public int GroupSubsetIndex;

        [FieldOffset(12)] public Sphere Bounds;

        public void Read(BinaryObjectReader reader)
        {
            Type = (NodeType)reader.ReadInt32();
            LeftIndex = reader.ReadInt32();
            RightIndex = reader.ReadInt32();
            Bounds = reader.ReadObjectOffset<Sphere>();
        }

        public readonly void Write(BinaryObjectWriter writer)
        {
            writer.WriteInt32((int)Type);
            writer.WriteInt32(LeftIndex);
            writer.WriteInt32(RightIndex);
            writer.WriteObjectOffset(Bounds);
        }
    }

    private enum NodeType : int
    {
        Branch,
        Leaf
    }
}