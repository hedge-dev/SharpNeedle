namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;

using SharpNeedle.Resource;
using System;

public abstract class BaseShaderPermutation<P, S> : IBinarySerializable
    where P : struct, Enum
    where S : BaseShader, new()
{
    public abstract string ShaderFileExtension { get; }

    public P SubPermutations { get; set; }
    public string PermutationName { get; set; }
    public ResourceReference<S> Shader { get; set; }

    public virtual void Read(BinaryObjectReader reader)
    {
        SubPermutations = (P)Enum.ToObject(typeof(P), reader.ReadUInt32());
        reader.ReadOffset(() => PermutationName = reader.ReadString(StringBinaryFormat.NullTerminated));
        reader.ReadOffset(() => Shader = reader.ReadString(StringBinaryFormat.NullTerminated));
    }

    public virtual void Write(BinaryObjectWriter writer)
    {
        writer.Write(Convert.ToUInt32(SubPermutations));
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, PermutationName, -1, 1);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Shader.Name, -1, 1);
    }

    public BaseShaderPermutation(P subPermutations, string permutationName, ResourceReference<S> shader)
    {
        SubPermutations = subPermutations;
        PermutationName = permutationName;
        Shader = shader;
    }

    public virtual void ResolveDependencies(IResourceResolver resolver)
    {
        if (Shader.IsValid())
        {
            return;
        }

        ResourceReference<S> shader = Shader;

        string filename = $"{Shader.Name}{ShaderFileExtension}";
        shader.Resource = resolver.Open<S>(filename)
            ?? throw new ResourceResolveException("Failed resolve shader", [filename]);

        Shader = shader;

        Shader.Resource!.ResolveDependencies(resolver);
    }

    public virtual void WriteDependencies(IDirectory directory)
    {
        if (Shader.IsValid())
        {
            using IFile file = directory.CreateFile($"{Shader.Name}{ShaderFileExtension}");
            Shader.Resource!.Write(file);
            Shader.Resource.WriteDependencies(directory);
        }
    }
}
