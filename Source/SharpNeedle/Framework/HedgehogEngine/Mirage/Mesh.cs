namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

using SharpNeedle.Structs;
using System.Text.Json.Serialization;

public class Mesh : IBinarySerializable<uint>, IDisposable, ICloneable<Mesh>
{
    public ResourceReference<Material> Material { get; set; }
    public ushort[] Faces { get; set; } = [];
    public uint VertexSize { get; set; }
    public uint VertexCount { get; set; }
    public byte[] Vertices { get; set; } = [];
    public short[] BoneIndices { get; set; } = [];
    public List<VertexElement> Elements { get; set; } = [];
    public List<TextureUnit> Textures { get; set; } = [];
    public MeshSlot Slot { get; set; }

    public void Read(BinaryObjectReader reader, uint version)
    {
        Elements = [];
        Material = reader.ReadStringOffsetOrEmpty();

        Faces = reader.ReadArrayOffset<ushort>(reader.Read<int>());
        VertexCount = reader.Read<uint>();
        VertexSize = reader.Read<uint>();
        Vertices = reader.ReadArrayOffset<byte>((int)(VertexCount * VertexSize));

        reader.ReadOffset(() =>
        {
            while (true)
            {
                VertexElement element = reader.Read<VertexElement>();
                if (element.Format == VertexFormat.Invalid)
                {
                    break;
                }

                Elements.Add(element);
                reader.Align(4);
            }
        });

        int boneCount = reader.ReadInt32();

        if (boneCount == 0)
        {
            BoneIndices = [];
            reader.ReadOffsetValue();
        }
        else if (version >= 6)
        {
            BoneIndices = reader.ReadArrayOffset<short>(boneCount);
        }
        else
        {
            BoneIndices = reader.ReadArrayOffset<byte>(boneCount).Select(Convert.ToInt16).ToArray();
        }

        Textures = reader.ReadObject<BinaryList<BinaryPointer<TextureUnit>>>().Unwind();
        SwapVertexEndianness();
    }

    public void Write(BinaryObjectWriter writer, uint version)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Path.GetFileNameWithoutExtension(Material.Name));
        writer.Write(Faces.Length);
        writer.WriteArrayOffset(Faces);

        byte[] verticesClone = new byte[Vertices.Length];
        Array.Copy(Vertices, verticesClone, verticesClone.LongLength);
        VertexElement.SwapEndianness([.. Elements], verticesClone.AsSpan(), (nint)VertexCount, (nint)VertexSize);

        writer.Write(VertexCount);
        writer.Write(VertexSize);
        writer.WriteArrayOffset(verticesClone);

        writer.WriteOffset(() =>
        {
            foreach (VertexElement element in Elements)
            {
                writer.Write(element);
                writer.Align(4);
            }

            writer.Write(VertexElement.Invalid);
        });

        writer.Write(BoneIndices.Length);
        if (BoneIndices.Length == 0)
        {
            writer.WriteOffsetValue(0);
        }
        else
        if (version >= 6)
        {
            writer.WriteArrayOffset(BoneIndices);
        }
        else
        {
            writer.WriteArrayOffset(BoneIndices.Select(Convert.ToByte).ToArray());
        }

        writer.Write(Textures.Count);
        if (Textures.Count == 0)
        {
            writer.WriteOffsetValue(0);
        }
        else
        {
            writer.WriteOffset(() =>
            {
                foreach (TextureUnit texture in Textures)
                {
                    writer.WriteObjectOffset(texture);
                }
            });
        }
    }

    public void SwapVertexEndianness()
    {
        if (Vertices.Length == 0)
        {
            return;
        }

        VertexElement.SwapEndianness([.. Elements], Vertices.AsSpan(), (nint)VertexCount, (nint)VertexSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref byte GetVertexReference(int idx)
    {
        return ref Vertices[idx * VertexSize];
    }

    public void ResolveDependencies(IResourceResolver resolver)
    {
        if (Material.IsValid())
        {
            return;
        }

        string resource = $"{Material.Name}.material";
        Material = resolver.Open<Material>(resource)
            ?? throw new ResourceResolveException("Failed to resolve Material", [resource]);
    }

    public void WriteDependencies(IDirectory dir)
    {
        if (!Material.IsValid())
        {
            return;
        }

        IFile materialFile = dir.CreateFile($"{Material.Name}.material");
        Material.Resource!.Write(materialFile);
        Material.Resource.WriteDependencies(dir);
    }

    public Mesh Clone()
    {
        return new()
        {
            BoneIndices = BoneIndices,
            Elements = Elements,
            Faces = Faces,
            Material = Material,
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
        Material.Resource?.Dispose();
        Material = default;
        Vertices = [];
        Elements = [];
    }
}

public struct MeshSlot
{
    [JsonInclude]
    public Mesh.Type Type;

    [JsonInclude]
    public readonly string? Name;

    [JsonConstructor]
    internal MeshSlot(Mesh.Type type, string name)
    {
        Type = type;

        if (type == Mesh.Type.Special)
        {
            Name = name;
        }
    }

    public MeshSlot(string name)
    {
        Name = default;
        switch (name.ToLowerInvariant())
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
    {
        return Name ?? Type.ToString();
    }

    public readonly override bool Equals(object? obj)
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
        return Type == Mesh.Type.Special ? (Name?.GetHashCode() ?? 0) : Type.GetHashCode();
    }

    public static bool operator ==(MeshSlot lhs, Mesh.Type rhs)
    {
        return lhs.Type == rhs;
    }

    public static bool operator !=(MeshSlot lhs, Mesh.Type rhs)
    {
        return lhs.Type != rhs;
    }

    public static implicit operator MeshSlot(Mesh.Type type)
    {
        return new(type);
    }

    public static implicit operator MeshSlot(string name)
    {
        return new(name);
    }
}