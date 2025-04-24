namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;

using SharpNeedle.Resource;
using System;
using System.Collections.ObjectModel;

public abstract class BaseShaderPermutation<TSubPermutation, TSubPermutationFlag, TShader> : IBinarySerializable
    where TSubPermutation : struct, Enum
    where TSubPermutationFlag : struct, Enum
    where TShader : BaseShader, new()
{
    private TSubPermutation _subPermutations;
    private string _shaderName = string.Empty;
    private readonly Dictionary<TSubPermutationFlag, ResourceReference<TShader>> _shaders;

    public abstract string ShaderFileExtension { get; }
    public abstract TSubPermutationFlag SubPermutationFlagMask { get; }

    public TSubPermutation SubPermutations
    {
        get => _subPermutations;
        set
        {
            if (_subPermutations.Equals(value))
            {
                return;
            }

            _subPermutations = value;
            UpdateShaderDictionary();
        }
    }

    public string PermutationName { get; set; }

    public string ShaderName
    {
        get => _shaderName;
        set
        {
            if (_shaderName == value)
            {
                return;
            }

            _shaderName = value;
            UpdateShaderDictionary();
        }
    }

    public ReadOnlyDictionary<TSubPermutationFlag, ResourceReference<TShader>> Shaders { get; }

    public BaseShaderPermutation(TSubPermutation subPermutations, string permutationName, string shaderName)
    {
        _shaders = [];
        Shaders = new(_shaders);

        _subPermutations = subPermutations;
        PermutationName = permutationName;
        _shaderName = shaderName;

        UpdateShaderDictionary();
    }

    private static T UIntToFlag<T>(uint value) where T : struct, Enum
    {
        return (T)Enum.ToObject(typeof(T), value);
    }

    public virtual void Read(BinaryObjectReader reader)
    {
        _subPermutations = UIntToFlag<TSubPermutation>(reader.ReadUInt32());
        reader.ReadOffset(() => PermutationName = reader.ReadString(StringBinaryFormat.NullTerminated));
        reader.ReadOffset(() => _shaderName = reader.ReadString(StringBinaryFormat.NullTerminated));
        UpdateShaderDictionary();
    }

    public virtual void Write(BinaryObjectWriter writer)
    {
        writer.Write(Convert.ToUInt32(SubPermutations));
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, PermutationName, -1, 1);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ShaderName, -1, 1);
    }

    private void UpdateShaderDictionary()
    {
        if (SubPermutations.Equals(default(TSubPermutation)))
        {
            _shaders.Clear();
            return;
        }

        Dictionary<TSubPermutationFlag, ResourceReference<TShader>> shaders = [];
        uint permutations = Convert.ToUInt32(SubPermutations);

        uint flag = 0;
        while (permutations != 0)
        {
            if ((permutations & 1) != 0)
            {
                TSubPermutationFlag key = UIntToFlag<TSubPermutationFlag>(flag);

                if (!_shaders.TryGetValue(key, out ResourceReference<TShader> shader))
                {
                    shader = ShaderName;

                    if (flag != 0)
                    {
                        TSubPermutation permutation = UIntToFlag<TSubPermutation>(1u << (int)flag);
                        shader += "_" + permutation.ToString();
                    }
                }

                shaders[key] = shader;
            }

            permutations >>= 1;
            flag++;
        }

        _shaders.Clear();

        foreach (KeyValuePair<TSubPermutationFlag, ResourceReference<TShader>> item in shaders)
        {
            _shaders.Add(item.Key, item.Value);
        }
    }

    public virtual void ResolveDependencies(IResourceResolver resolver)
    {
        List<string> unresolvedResources = [];

        foreach (TSubPermutationFlag key in _shaders.Keys)
        {
            ResourceReference<TShader> shader = _shaders[key];

            if (shader.IsValid())
            {
                continue;
            }

            string filename = $"{shader.Name}{ShaderFileExtension}";

            if (resolver.Open<TShader>(filename) is TShader resource)
            {
                shader.Resource = resource;
            }
            else
            {
                unresolvedResources.Add(filename);
                continue;
            }

            _shaders[key] = shader;

            shader.Resource!.ResolveDependencies(resolver);
        }

        if (unresolvedResources.Count > 0)
        {
            throw new ResourceResolveException("Failed to resolve shaders", [.. unresolvedResources]);
        }
    }

    public virtual void WriteDependencies(IDirectory directory)
    {
        foreach (ResourceReference<TShader> shader in _shaders.Values)
        {
            if (!shader.IsValid())
            {
                continue;
            }

            using IFile file = directory.CreateFile($"{shader.Name}{ShaderFileExtension}");
            shader.Resource!.Write(file);
            shader.Resource!.WriteDependencies(directory);
        }
    }
}
