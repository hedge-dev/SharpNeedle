namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

using SharpNeedle.Structs;

[NeedleResource("hh/tbst", @"\.tbst$")]
public class TerrainBlockSphereTree : SampleChunkResource
{
    public new BVHNode<Sphere, (int GroupID, int SubsetID)> Root { get; set; } = new(default);

    public static TerrainBlockSphereTree Build(List<TerrainGroup> groups)
    {
        List<KeyValuePair<Sphere, (int GroupID, int SubsetID)>> nodes = new(groups.Capacity);

        for(int groupId = 0; groupId < groups.Count; groupId++)
        {
            TerrainGroup group = groups[groupId];
            for(int subsetID = 0; subsetID < group.Subsets.Count; subsetID++)
            {
                TerrainGroup.Subset set = group.Subsets[subsetID];
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
        reader.Read(out int nodeCount);
        BinaryNode[] nodes = new BinaryNode[nodeCount];
        reader.ReadOffset(() =>
        {
            for(int i = 0; i < nodeCount; i++)
            {
                nodes[i] = reader.ReadObjectOffset<BinaryNode>();
            }
        });
        reader.Read(out int rootIndex);
        Root = BuildNode(ref nodes[rootIndex]);

        BVHNode<Sphere, (int, int)> BuildNode(ref BinaryNode node)
        {
            BVHNode<Sphere, (int GroupID, int SubsetID)> result = new(node.Bounds);

            if(node.Type == NodeType.Branch)
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

        writer.Write(nodes.Count);
        writer.WriteOffset(() =>
        {
            foreach(BinaryNode node in nodes)
            {
                writer.WriteOffset(() => writer.WriteObject(node));
            }
        });
        writer.Write(nodes.Count - 1);

        static void BuildNodes(BVHNode<Sphere, (int, int)> node, List<BinaryNode> result)
        {
            BinaryNode bNode = new()
            {
                Bounds = node.Key,
                Type = (NodeType)node.Type
            };

            if(node.Type == BVHNodeType.Leaf)
            {
                bNode.TerrainGroupIndex = node.Value.Item1;
                bNode.GroupSubsetIndex = node.Value.Item2;
            }
            else
            {
                if(node.Left != null)
                {
                    bNode.LeftIndex = result.Count;
                    BuildNodes(node.Left, result);
                }

                if(node.Right != null)
                {
                    bNode.RightIndex = result.Count;
                    BuildNodes(node.Right, result);
                }
            }

            result.Add(bNode);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    internal struct BinaryNode : IBinarySerializable
    {
        [FieldOffset(0)] public NodeType Type;
        [FieldOffset(4)] public int LeftIndex;
        [FieldOffset(8)] public int RightIndex;

        [FieldOffset(4)] public int TerrainGroupIndex;
        [FieldOffset(8)] public int GroupSubsetIndex;

        [FieldOffset(12)] public Sphere Bounds;

        public void Read(BinaryObjectReader reader)
        {
            Type = reader.Read<NodeType>();
            LeftIndex = reader.Read<int>();
            RightIndex = reader.Read<int>();
            Bounds = reader.ReadValueOffset<Sphere>();
        }

        public readonly void Write(BinaryObjectWriter writer)
        {
            writer.Write(Type);
            writer.Write(LeftIndex);
            writer.Write(RightIndex);
            writer.WriteValueOffset(Bounds);
        }
    }

    public enum NodeType : int
    {
        Branch,
        Leaf
    }
}