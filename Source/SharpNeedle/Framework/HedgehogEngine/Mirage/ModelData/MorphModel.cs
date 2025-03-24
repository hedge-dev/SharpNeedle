﻿namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

using SharpNeedle.IO;
using SharpNeedle.Resource;
using SharpNeedle.Utilities;

public class MorphModel : IBinarySerializable<uint>
{
    public List<MorphTarget> Targets { get; set; } = [];
    public MeshGroup? MeshGroup { get; set; }

    public void Read(BinaryObjectReader reader, uint version)
    {
        reader.Read(out uint vertexCount);
        long vertexOffset = reader.ReadOffsetValue();

        uint flags = reader.Read<uint>();
        if (flags != 1)
        {
            throw new Exception($"{flags} is not 1! Report this model!");
        }

        reader.Read(out uint shapeCount);
        Targets = new List<MorphTarget>((int)shapeCount);
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
                mesh.Positions = reader.ReadArrayOffset<Vector3>((int)vertexCount);
            }
        });

        MeshGroup = reader.ReadObject<MeshGroup, uint>(version);

        byte[] vertexData = [];

        for (int i = 0; i < MeshGroup.Count; i++)
        {
            Mesh mesh = MeshGroup[i];

            if (i == 0)
            {
                vertexData = reader.ReadArrayAtOffset<byte>(vertexOffset, (int)(vertexCount * mesh.VertexSize));
            }

            mesh.VertexCount = vertexCount;
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

        writer.Write(mesh.VertexCount);
        writer.WriteArrayOffset(verticesClone);

        writer.Write(1); // Some kind of flags
        writer.Write(Targets.Count);

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
                writer.WriteArrayOffset(shape.Positions);
            }
        });

        writer.WriteObject(MeshGroup, version);
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