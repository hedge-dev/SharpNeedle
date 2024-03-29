﻿namespace SharpNeedle.HedgehogEngine.Mirage;

public class MorphModel : SampleChunkResource
{
    public List<MorphTarget> Targets { get; set; }
    public Mesh Mesh { get; set; }

    public override void Read(BinaryObjectReader reader)
    {
        reader.Read(out uint vertexCount);
        var vertexOffset = reader.ReadOffsetValue();
        
        var flags = reader.Read<uint>();
        if (flags != 1)
            throw new Exception($"{flags} is not 1! Report this model!");

        reader.Read(out uint shapeCount);
        Targets = new List<MorphTarget>((int)shapeCount);
        for (uint i = 0; i < shapeCount; i++)
            Targets.Add(new());
        
        reader.ReadOffset(() =>
        {
            for (int i = 0; i < shapeCount; i++)
                Targets[i].Name = reader.ReadStringOffset();
        });

        reader.ReadOffset(() =>
        {
            foreach (var mesh in Targets)
                mesh.Positions = reader.ReadArrayOffset<Vector3>((int)vertexCount);
        });
        
        Mesh = reader.ReadObject<MeshGroup>().FirstOrDefault();
        if (Mesh != null)
        {
            reader.ReadAtOffset(vertexOffset, () =>
            {
                Mesh.VertexCount = vertexCount;
                Mesh.Vertices = reader.ReadArray<byte>((int)(vertexCount * Mesh.VertexSize));
                Mesh.SwapVertexEndianness();
            });
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        var verticesClone = new byte[Mesh.Vertices.Length];
        Array.Copy(Mesh.Vertices, verticesClone, verticesClone.LongLength);
        VertexElement.SwapEndianness(Mesh.Elements.ToArray(), verticesClone.AsSpan(), (nint)Mesh.VertexCount, (nint)Mesh.VertexSize);

        writer.Write(Mesh.VertexCount);
        writer.WriteArrayOffset(verticesClone);
        
        verticesClone = null;
        writer.Write(1); // Some kind of flags
        writer.Write(Targets.Count);

        writer.WriteOffset(() =>
        {
            foreach (var shape in Targets)
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, shape.Name);
        });

        writer.WriteOffset(() =>
        {
            foreach (var shape in Targets)
                writer.WriteArrayOffset(shape.Positions);
        });

        var dummyMesh = Mesh.Clone();
        dummyMesh.Vertices = Array.Empty<byte>();
        dummyMesh.VertexCount = 0;

        var dummyGroup = new MeshGroup { dummyMesh };
        writer.WriteObject(dummyGroup);
    }
}

public class MorphTarget
{
    public string Name { get; set; }
    public Vector3[] Positions { get; set; }

    public override string ToString()
    {
        return Name ?? base.ToString();
    }
}