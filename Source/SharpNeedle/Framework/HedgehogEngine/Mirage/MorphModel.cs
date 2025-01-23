namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

using SharpNeedle.IO;
using SharpNeedle.Resource;
using SharpNeedle.Utilities;

public class MorphModel : SampleChunkResource
{
    public List<MorphTarget> Targets { get; set; } = [];
    public MeshGroup? Meshgroup { get; set; }

    public override void Read(BinaryObjectReader reader)
    {
        reader.Read(out uint vertexCount);
        long vertexOffset = reader.ReadOffsetValue();

        uint flags = reader.Read<uint>();
        if(flags != 1)
        {
            throw new Exception($"{flags} is not 1! Report this model!");
        }

        reader.Read(out uint shapeCount);
        Targets = new List<MorphTarget>((int)shapeCount);
        for(uint i = 0; i < shapeCount; i++)
        {
            Targets.Add(new());
        }

        reader.ReadOffset(() =>
        {
            for(int i = 0; i < shapeCount; i++)
            {
                Targets[i].Name = reader.ReadStringOffsetOrEmpty();
            }
        });

        reader.ReadOffset(() =>
        {
            foreach(MorphTarget mesh in Targets)
            {
                mesh.Positions = reader.ReadArrayOffset<Vector3>((int)vertexCount);
            }
        });

        Meshgroup = reader.ReadObject<MeshGroup>();
        byte[] vertexData = [];

        for(int i = 0; i < Meshgroup.Count; i++)
        {
            Mesh mesh = Meshgroup[i];

            if(i == 0)
            {
                vertexData = reader.ReadArrayAtOffset<byte>(vertexOffset, (int)(vertexCount * mesh.VertexSize));
            }

            mesh.VertexCount = vertexCount;
            mesh.Vertices = vertexData;

            if(i == 0)
            {
                mesh.SwapVertexEndianness();
            }
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        if(Meshgroup == null)
        {
            throw new InvalidOperationException("Meshgroup is null");
        }

        if(Meshgroup.Count == 0)
        {
            throw new InvalidOperationException("Meshgroup has no meshes");
        }

        Mesh mesh = Meshgroup.First();

        byte[] verticesClone = new byte[mesh.Vertices.Length];
        Array.Copy(mesh.Vertices, verticesClone, verticesClone.LongLength);
        VertexElement.SwapEndianness([.. mesh.Elements], verticesClone.AsSpan(), (nint)mesh.VertexCount, (nint)mesh.VertexSize);

        writer.Write(mesh.VertexCount);
        writer.WriteArrayOffset(verticesClone);

        writer.Write(1); // Some kind of flags
        writer.Write(Targets.Count);

        writer.WriteOffset(() =>
        {
            foreach(MorphTarget shape in Targets)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, shape.Name);
            }
        });

        writer.WriteOffset(() =>
        {
            foreach(MorphTarget shape in Targets)
            {
                writer.WriteArrayOffset(shape.Positions);
            }
        });

        MeshGroup dummyGroup = [..Meshgroup.Select(x =>
        {
            Mesh dummyMesh = x.Clone();
            dummyMesh.Vertices = [];
            dummyMesh.VertexCount = 0;
            return dummyMesh;
        })];

        dummyGroup.Name = Meshgroup.Name;

        writer.WriteObject(Meshgroup);
    }

    public override void ResolveDependencies(IResourceResolver dir)
    {
        Meshgroup?.ResolveDependencies(dir);
    }

    public override void WriteDependencies(IDirectory dir)
    {
        Meshgroup?.WriteDependencies(dir);
    }
}

public class MorphTarget
{
    public string? Name { get; set; }
    public Vector3[] Positions { get; set; } = [];

    public override string ToString()
    {
        return Name ?? base.ToString()!;
    }
}