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
            while (true)
            {
                var element = reader.Read<VertexElement>();
                if (element.Format == VertexFormat.Invalid)
                    break;

                Elements.Add(element);
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
            {
                writer.Write(element);
                writer.Align(4);
            }

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

    public void ResolveDependencies(IResourceResolver resolver)
    {
        if (Material != null)
            return;

        Material = resolver.Open<Material>($"{MaterialName}.material");
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
        Transparent,
        PunchThrough,
        Special
    }

    public void Dispose()
    {
        Material?.Dispose();
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

            case "trans":
            case "transparent":
                Type = Mesh.Type.Transparent;
                break;

            case "punch":
            case "punchthrough":
                Type = Mesh.Type.PunchThrough;
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

    public readonly override string ToString()
        => Name ?? Type.ToString();

    public readonly override bool Equals(object obj)
    {
        if (obj is Mesh.Type type)
        {
            return this == type;
        }
        else if (obj is MeshSlot slot)
        {
            if (slot.Type == Mesh.Type.Special && Type == Mesh.Type.Special)
            {
                return slot.Name == Name;
            }

            return slot.Type == Type;
        }

        return false;
    }

    public readonly override int GetHashCode()
    {
        return Type == Mesh.Type.Special ? Name.GetHashCode() : Type.GetHashCode();
    }

    public static bool operator ==(MeshSlot lhs, Mesh.Type rhs)
        => lhs.Type == rhs;

    public static bool operator !=(MeshSlot lhs, Mesh.Type rhs) 
        => lhs.Type != rhs;

    public static implicit operator MeshSlot(Mesh.Type type) => new(type);
    public static implicit operator MeshSlot(string name) => new(name);
}