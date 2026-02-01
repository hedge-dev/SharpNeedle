namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

using SharpNeedle.IO;
using SharpNeedle.Resource;
using SharpNeedle.Utilities;

public class MorphModel : IBinarySerializable<uint>
{
    public List<MorphTarget> Targets { get; set; } = [];
    public MeshGroup? MeshGroup { get; set; }

    public void Read(BinaryObjectReader reader, uint version)
    {
        int vertexCount = reader.ReadInt32();
        long vertexOffset = reader.ReadOffsetValue();

        uint flags = reader.ReadUInt32();
        if (flags != 1)
        {
            throw new Exception($"{flags} is not 1! Report this model!");
        }

        int shapeCount = reader.ReadInt32();
        Targets = new List<MorphTarget>(shapeCount);
        for (uint i = 0; i < shapeCount; i++)
        {
            Targets.Add(new());
        }

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < shapeCount; i++)
            {
                Targets[i].Name = reader.ReadStringOffsetOrEmpty();
            }
        });

        reader.ReadOffset(() =>
        {
            foreach (MorphTarget mesh in Targets)
            {
                reader.ReadOffset(() => mesh.Positions =  reader.ReadVector3Array(vertexCount));
            }
        });

        MeshGroup = reader.ReadObject<MeshGroup, MeshSerializeContext>(new(version, true));

        byte[] vertexData = [];

        for (int i = 0; i < MeshGroup.Count; i++)
        {
            Mesh mesh = MeshGroup[i];

            if (i == 0)
            {
                vertexData = reader.ReadArrayAtOffset<byte>(vertexOffset, (int)(vertexCount * mesh.VertexSize));
            }

            mesh.VertexCount = (uint)vertexCount;
            mesh.Vertices = vertexData;

            if (i == 0)
            {
                mesh.SwapVertexEndianness();
            }
        }
    }

    public void Write(BinaryObjectWriter writer, uint version)
    {
        if (MeshGroup == null)
        {
            throw new InvalidOperationException("Meshgroup is null");
        }

        if (MeshGroup.Count == 0)
        {
            throw new InvalidOperationException("Meshgroup has no meshes");
        }

        Mesh mesh = MeshGroup.First();

        byte[] verticesClone = new byte[mesh.Vertices.Length];
        Array.Copy(mesh.Vertices, verticesClone, verticesClone.LongLength);
        VertexElement.SwapEndianness([.. mesh.Elements], verticesClone.AsSpan(), (nint)mesh.VertexCount, (nint)mesh.VertexSize);

        writer.WriteUInt32(mesh.VertexCount);
        writer.WriteArrayOffset(verticesClone);

        writer.WriteInt32(1); // Some kind of flags
        writer.WriteInt32(Targets.Count);

        writer.WriteOffset(() =>
        {
            foreach (MorphTarget shape in Targets)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, shape.Name);
            }
        });

        writer.WriteOffset(() =>
        {
            foreach (MorphTarget shape in Targets)
            {
                writer.WriteOffset(() => writer.WriteVector3Array(shape.Positions));
            }
        });

        writer.WriteObject(MeshGroup, new MeshSerializeContext(version, true));
    }

    public void ResolveDependencies(IResourceResolver dir)
    {
        MeshGroup?.ResolveDependencies(dir);
    }

    public void WriteDependencies(IDirectory dir)
    {
        MeshGroup?.WriteDependencies(dir);
    }
}