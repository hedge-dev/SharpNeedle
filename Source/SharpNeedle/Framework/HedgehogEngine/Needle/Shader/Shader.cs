namespace SharpNeedle.Framework.HedgehogEngine.Needle.Shader;

using SharpNeedle.IO;
using SharpNeedle.Utilities;

[NeedleResource("hh/needle/shader-list", @"\.[pvc]so$")]
public class Shader : ResourceBase
{
    public static readonly ulong FileSignature = BinaryHelper.MakeSignature<ulong>("HHNEEDLE");
    public static readonly ulong ShaderSignature = BinaryHelper.MakeSignature<ulong>("HNSHV002");

    public struct Feature : IBinarySerializable
    {
        private string? _name;

        public int Unknown { get; set; }

        public string Name
        {
            readonly get => _name ?? string.Empty;
            set => _name = value;
        }

        public void Read(BinaryObjectReader reader)
        {
            Unknown = reader.ReadInt32();
            Name = reader.ReadString(StringBinaryFormat.NullTerminated);
            reader.Align(4);
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteInt32(Unknown);
            writer.WriteString(StringBinaryFormat.NullTerminated, Name);
            writer.Align(4);
        }

        public override string ToString()
        {
            return $"{Name}: {Unknown}";
        }
    }

    public string BuildPath { get; set; } = string.Empty;

    // Unused and never filled
    public string Unknown { get; set; } = string.Empty;

    public List<Feature> Features { get; set; } = [];

    public List<int> PermutationVariant { get; set; } = [];

    public List<ShaderVariant> ShaderVariants { get; set; } = []; 

    public override void Read(IFile file)
    {
        BaseFile = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);
        using BinaryObjectReader reader = new(file.Open(), StreamOwnership.Transfer, Endianness.Little);

        BinaryHelper.EnsureSignature(reader.ReadNative<ulong>(), true, FileSignature);
        reader.Skip(4); // file size

        BuildPath = reader.ReadString(StringBinaryFormat.NullTerminated);
        reader.Align(4);

        BinaryHelper.EnsureSignature(reader.ReadNative<ulong>(), true, ShaderSignature);
        reader.Skip(4); // shader data size

        Unknown = reader.ReadString(StringBinaryFormat.NullTerminated);
        reader.Align(4);

        reader.Endianness = Endianness.Big;
        int permutationCount = reader.ReadInt32();
        Features = [.. reader.ReadObjectArray<Feature>(permutationCount)];

        int variantCount = 1 << Features.Count;
        PermutationVariant = [.. reader.ReadArray<int>(variantCount)];

        int shaderVariantCount = reader.ReadInt32();
        ShaderVariants = [.. reader.ReadObjectArray<ShaderVariant>(shaderVariantCount)];
    }

    public override void Write(IFile file)
    {
        BaseFile = file;
        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Little);

        writer.WriteUInt64(FileSignature);

        long fileDataStart = writer.Position;
        SeekToken fileSizeToken = writer.At();
        writer.WriteUInt32(0);

        writer.WriteString(StringBinaryFormat.NullTerminated, BuildPath);
        writer.Align(4);

        writer.WriteUInt64(ShaderSignature);
        long shaderDataStart = writer.Position;
        SeekToken shaderSizeToken = writer.At();
        writer.WriteUInt32(0);

        writer.WriteString(StringBinaryFormat.NullTerminated, Unknown);
        writer.Align(4);

        writer.Endianness = Endianness.Big;
        writer.WriteInt32(Features.Count);
        writer.WriteObjectCollection(Features);

        int variantCount = 1 << PermutationVariant.Count;
        if(PermutationVariant.Count != variantCount)
        {
            throw new InvalidOperationException($"Shader is supposed to have {variantCount} variants, but has {PermutationVariant.Count}!");
        }

        writer.WriteArray(PermutationVariant.ToArray());

        writer.WriteInt32(ShaderVariants.Count);
        writer.WriteObjectCollection(ShaderVariants);

        long dataEnd = writer.Position;
        using(SeekToken temp = writer.At())
        {
            fileSizeToken.Dispose();
            writer.Write((int)(dataEnd - fileDataStart));
        }

        using(SeekToken temp = writer.At())
        {
            shaderSizeToken.Dispose();
            writer.Write((int)(dataEnd - shaderDataStart));
        }
    }
}
