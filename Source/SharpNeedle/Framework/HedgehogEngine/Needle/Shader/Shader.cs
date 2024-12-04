﻿namespace SharpNeedle.Framework.HedgehogEngine.Needle.Shader;

using SharpNeedle.IO;
using SharpNeedle.Utilities;

[NeedleResource("hhn/shader-list", @"\.[pvc]so$")]
public class Shader : ResourceBase
{
    public static readonly ulong FileSignature = BinaryHelper.MakeSignature<ulong>("HHNEEDLE");
    public static readonly ulong ShaderSignature = BinaryHelper.MakeSignature<ulong>("HNSHV002");

    public struct Permutation : IBinarySerializable
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

    /// <summary>
    /// Might just be padding, yet to be determined
    /// </summary>
    public uint Unknown1 { get; set; }

    public List<Permutation> Permutations { get; set; } = [];

    /// <summary>
    /// Might just be padding, yet to be determined
    /// </summary>
    public uint Unknown2 { get; set; }

    /// <summary>
    /// One-based index to <see cref="ShaderVariants"/>. 
    /// <br/> 0 Probably means "null" here, so 1 is used to access index 0.
    /// </summary>
    public List<int> Variants { get; set; } = [];

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

        reader.Endianness = Endianness.Big;
        Unknown1 = reader.ReadUInt32();
        int permutationCount = reader.ReadInt32();
        Permutations = [.. reader.ReadObjectArray<Permutation>(permutationCount)];

        Unknown2 = reader.ReadUInt32();
        int variantCount = 1 << Permutations.Count;
        Variants = [.. reader.ReadArray<int>(variantCount)];

        int shaderVariantCount = Variants.Max();
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

        writer.Endianness = Endianness.Big;
        writer.WriteUInt32(Unknown1);
        writer.WriteInt32(Permutations.Count);
        writer.WriteObjectCollection(Permutations);

        writer.WriteUInt32(Unknown2);

        int variantCount = 1 << Variants.Count;
        if(Variants.Count != variantCount)
        {
            throw new InvalidOperationException($"Shader is supposed to have {variantCount} variants, but has {Variants.Count}!");
        }

        writer.WriteArray(Variants.ToArray());

        int shaderVariantCount = Variants.Max();
        if(ShaderVariants.Count != shaderVariantCount)
        {
            throw new InvalidOperationException($"Shader uses up to {shaderVariantCount} variants, but only has {ShaderVariants.Count} shader variants!");
        }

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
