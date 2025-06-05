﻿namespace SharpNeedle.Framework.HedgehogEngine.Needle.Shader;

using SharpNeedle.IO;
using SharpNeedle.Utilities;

[NeedleResource("hh/needle/shader-list", @"\.[pvc]so$")]
public class Shader : ResourceBase, IBinarySerializable
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

    public List<int> Permutations { get; set; } = [];

    public List<ShaderVariant> Variants { get; set; } = [];

    public override void Read(IFile file)
    {
        BaseFile = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);
        using BinaryObjectReader reader = new(file.Open(), StreamOwnership.Transfer, Endianness.Little);
        Read(reader);
    }

    public void Read(BinaryObjectReader reader)
    {
        BinaryHelper.EnsureSignature(reader.ReadNative<ulong>(), true, FileSignature);
        reader.Skip(4); // file size

        BuildPath = reader.ReadString(StringBinaryFormat.NullTerminated);
        reader.Align(4);

        BinaryHelper.EnsureSignature(reader.ReadNative<ulong>(), true, ShaderSignature);
        reader.Skip(4); // shader data size

        Unknown = reader.ReadString(StringBinaryFormat.NullTerminated);
        reader.Align(4);

        reader.Endianness = Endianness.Big;
        int featureCount = reader.ReadInt32();
        Features = [.. reader.ReadObjectArray<Feature>(featureCount)];

        int permutationCount = 1 << Features.Count;
        Permutations = [.. reader.ReadArray<int>(permutationCount)];

        int variantCount = reader.ReadInt32();
        Variants = [.. reader.ReadObjectArray<ShaderVariant>(variantCount)];
    }

    public override void Write(IFile file)
    {
        BaseFile = file;
        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Little);
        Write(writer);
    }

    public void Write(BinaryObjectWriter writer)
    { 
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

        int variantCount = 1 << Features.Count;
        if (Permutations.Count != variantCount)
        {
            throw new InvalidOperationException($"Shader is supposed to have {variantCount} permutations, but has {Permutations.Count}!");
        }

        writer.WriteArray(Permutations.ToArray());

        writer.WriteInt32(Variants.Count);
        writer.WriteObjectCollection(Variants);

        long dataEnd = writer.Position;
        using (SeekToken temp = writer.At())
        {
            fileSizeToken.Dispose();
            writer.Write((int)(dataEnd - fileDataStart));
        }

        using (SeekToken temp = writer.At())
        {
            shaderSizeToken.Dispose();
            writer.Write((int)(dataEnd - shaderDataStart));
        }
    }
}
