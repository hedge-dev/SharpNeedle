namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

using SharpNeedle.Framework.HedgehogEngine.Mirage.MaterialData;
using SharpNeedle.Structs;

public class Mesh : IBinarySerializable<MeshSerializeContext>, IDisposable, ICloneable<Mesh>
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

    public void Read(BinaryObjectReader reader, MeshSerializeContext context)
    {
        Elements = [];
        Material = reader.ReadStringOffsetOrEmpty();

        Faces = reader.ReadArrayOffset<ushort>(reader.ReadInt32());
        VertexCount = reader.ReadUInt32();
        VertexSize = reader.ReadUInt32();
        Vertices = reader.ReadArrayOffset<byte>((int)(VertexCount * VertexSize));

        reader.ReadOffset(() =>
        {
            while (true)
            {
                VertexElement element = reader.ReadObject<VertexElement>();
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
        else if (context.Version >= 6)
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

    public void Write(BinaryObjectWriter writer, MeshSerializeContext context)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Path.GetFileNameWithoutExtension(Material.Name));
        writer.WriteInt32(Faces.Length);
        writer.WriteArrayOffset(Faces);

        writer.WriteUInt32(context.IsMorphModel ? 0 : VertexCount);
        writer.WriteUInt32(VertexSize);

        if (context.IsMorphModel)
        {
            writer.WriteOffset(() => { });
        }
        else
        {
            byte[] verticesClone = new byte[Vertices.Length];
            Array.Copy(Vertices, verticesClone, verticesClone.LongLength);
            VertexElement.SwapEndianness([.. Elements], verticesClone.AsSpan(), (nint)VertexCount, (nint)VertexSize);
            writer.WriteArrayOffset(verticesClone);
        }

        writer.WriteOffset(() =>
        {
            foreach (VertexElement element in Elements)
            {
                writer.WriteObject(element);
                writer.Align(4);
            }

            writer.WriteObject(VertexElement.Invalid);
        });

        writer.WriteInt32(BoneIndices.Length);
        if (BoneIndices.Length == 0)
        {
            writer.WriteOffsetValue(0);
        }
        else
        if (context.Version >= 6)
        {
            writer.WriteArrayOffset(BoneIndices);
        }
        else
        {
            writer.WriteArrayOffset(BoneIndices.Select(Convert.ToByte).ToArray());
        }

        writer.WriteInt32(Textures.Count);
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

    public void Dispose()
    {
        Material.Resource?.Dispose();
        Material = default;
        Vertices = [];
        Elements = [];
    }
}