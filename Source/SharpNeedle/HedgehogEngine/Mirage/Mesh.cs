namespace SharpNeedle.HedgehogEngine.Mirage;

public class Mesh : IBinarySerializable, IDisposable, ICloneable<Mesh>
{
    public Material Material { get; set; }
    public string MaterialName { get; set; }
    public ushort[] Faces { get; set; }
    public uint VertexSize { get; set; }
    public uint VertexCount { get; set; }
    public byte[] Vertices { get; set; }
    public byte[] BoneIndices { get; set; }
    public List<VertexElement> Elements { get; set; }
    public List<TextureUnit> Textures { get; set; }
    public MeshSlot Slot { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Elements ??= new List<VertexElement>();
        MaterialName = reader.ReadStringOffset();

        Faces = reader.ReadArrayOffset<ushort>(reader.Read<int>());
        VertexCount = reader.Read<uint>();
        VertexSize = reader.Read<uint>();
        Vertices = reader.ReadArrayOffset<byte>((int)(VertexCount * VertexSize));

        reader.ReadOffset(() =>
        {
            var element = reader.Read<VertexElement>();
            reader.Align(4);

            while (element.Format != VertexFormat.Invalid)
            {
                Elements.Add(element);

                element = reader.Read<VertexElement>();
                reader.Align(4);
            }
        });

        BoneIndices = reader.ReadArrayOffset<byte>(reader.Read<int>());
        Textures = reader.ReadObject<BinaryList<BinaryPointer<TextureUnit>>>().Unwind();
        SwapVertexEndianness();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, MaterialName);
        writer.Write(Faces.Length);
        writer.WriteArrayOffset(Faces);

        var verticesClone = new byte[Vertices.Length];
        Array.Copy(Vertices, verticesClone, verticesClone.LongLength);
        VertexElement.SwapEndianness(Elements.ToArray(), verticesClone.AsSpan(), (nint)VertexCount, (nint)VertexSize);
        
        writer.Write(VertexCount);
        writer.Write(VertexSize);
        writer.WriteArrayOffset(verticesClone);

        verticesClone = null;

        writer.WriteOffset(() =>
        {
            foreach (var element in Elements)
                writer.Write(element);
            
            writer.Write(VertexElement.Invalid);
        });

        writer.Write(BoneIndices.Length);
        writer.WriteArrayOffset(BoneIndices);

        writer.Write(Textures.Count);
        writer.WriteOffset(() =>
        {
            foreach (var texture in Textures)
                writer.WriteObjectOffset(texture);
        });
    }

    public void SwapVertexEndianness()
    {
        if (Vertices == null || Vertices.Length == 0)
            return;

        VertexElement.SwapEndianness(Elements.ToArray(), Vertices.AsSpan(), (nint)VertexCount, (nint)VertexSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref byte GetVertexReference(int idx)
        => ref Vertices[idx * VertexSize];

    public void ResolveDependencies(IDirectory dir)
    {
        if (Material != null)
            return;

        var file = dir[$"{MaterialName}.material"];
        if (file == null)
            return;

        Material = new Material();
        Material.Read(file);
        Material.ResolveDependencies(dir);
    }

    public Mesh Clone()
    {
        return new()
        {
            BoneIndices = BoneIndices,
            Elements = Elements,
            Faces = Faces,
            MaterialName = MaterialName,
            Slot = Slot,
            Textures = Textures,
            VertexCount = VertexCount,
            VertexSize = VertexSize,
            Vertices = Vertices
        };
    }

    public enum Type
    {
        Opaque,
        PunchThrough,
        Transparent,
        Special
    }

    public void Dispose()
    {
        Material = null;
        Vertices = null;
        Elements = null;
    }
}

public struct MeshSlot
{
    public Mesh.Type Type;
    public readonly string Name;

    public MeshSlot(string name)
    {
        Name = default;
        switch (name.ToLower())
        {
            case "opaq":
            case "opaque":
                Type = Mesh.Type.Opaque;
                break;

            case "punch":
            case "punchthrough":
                Type = Mesh.Type.PunchThrough;
                break;

            case "trans":
            case "transparent":
                Type = Mesh.Type.Transparent;
                break;

            default:
                Type = Mesh.Type.Special;
                Name = name;
                break;
        }
    }

    public MeshSlot(Mesh.Type type)
    {
        Name = default;
        Type = type;
    }

    public override string ToString()
        => Name ?? Type.ToString();

    public static bool operator ==(MeshSlot lhs, Mesh.Type rhs)
        => lhs.Type == rhs;

    public static bool operator !=(MeshSlot lhs, Mesh.Type rhs) 
        => lhs.Type != rhs;

    public static implicit operator MeshSlot(Mesh.Type type) => new(type);
    public static implicit operator MeshSlot(string name) => new(name);
}