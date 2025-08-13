namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;
public abstract class BaseShader : SampleChunkResource
{
    public abstract char ByteCodeFileExtensionCharacter { get; }
    public abstract string ParameterFileExtension { get; }

    public ResourceReference<ResourceRaw> ByteCode { get; set; }

    public ShaderByteCodePlatform ByteCodePlatform { get; set; } = ShaderByteCodePlatform.None;

    public List<ResourceReference<ShaderParameter>> Parameters { get; set; } = [];

    public override void Read(BinaryObjectReader reader)
    {
        ByteCode = reader.ReadStringOffsetOrEmpty();
        long paramCount = reader.ReadOffsetValue();
        Parameters = new((int)paramCount);

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < paramCount; i++)
            {
                Parameters.Add(reader.ReadStringOffsetOrEmpty());
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Path.GetFileNameWithoutExtension(ByteCode.Name));

        if (Parameters.Count == 0)
        {
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
            return;
        }

        writer.WriteOffsetValue(Parameters.Count);
        writer.WriteOffset(() =>
        {
            foreach (ResourceReference<ShaderParameter> parameter in Parameters)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Path.GetFileNameWithoutExtension(parameter.Name));
            }
        });
    }

    protected override void Reset()
    {
        ByteCode = default;
        ByteCodePlatform = ShaderByteCodePlatform.None;
        Parameters.Clear();
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        List<string> unresolved = [];

        if (!ByteCode.IsValid())
        {
            ResourceReference<ResourceRaw> byteCode = ByteCode;

            if (ByteCodePlatform is ShaderByteCodePlatform.None)
            {
                foreach (ShaderByteCodePlatform platform in Enum.GetValues<ShaderByteCodePlatform>())
                {
                    if (platform is ShaderByteCodePlatform.None)
                    {
                        continue;
                    }

                    string filename = $"{ByteCode.Name}.{(char)platform}{ByteCodeFileExtensionCharacter}u";
                    byteCode.Resource = resolver.Open<ResourceRaw>(filename);

                    if (byteCode.Resource == null)
                    {
                        continue;
                    }
                    else
                    {
                        ByteCode = byteCode;
                        ByteCodePlatform = platform;
                    }
                }

                if (ByteCodePlatform == ShaderByteCodePlatform.None)
                {
                    // fallback to windows for the platform
                    unresolved.Add($"{ByteCode.Name}.{(char)ShaderByteCodePlatform.Windows}{ByteCodeFileExtensionCharacter}u");
                }
            }
            else
            {
                string filename = $"{ByteCode.Name}.{(char)ByteCodePlatform}{ByteCodeFileExtensionCharacter}u";
                byteCode.Resource = resolver.Open<ResourceRaw>(filename);

                if (byteCode.Resource == null)
                {
                    unresolved.Add(filename);
                }
                else
                {
                    ByteCode = byteCode;
                }
            }
        }

        for (int i = 0; i < Parameters.Count; i++)
        {
            ResourceReference<ShaderParameter> parameter = Parameters[i];

            if (parameter.IsValid())
            {
                continue;
            }

            string filename = $"{parameter.Name}{ParameterFileExtension}";
            parameter.Resource = resolver.Open<ShaderParameter>(filename);

            if (parameter.Resource == null)
            {
                unresolved.Add(filename);
            }
            else
            {
                Parameters[i] = parameter;
            }
        }

        if (unresolved.Count > 0)
        {
            throw new ResourceResolveException($"Failed to resolve {unresolved.Count} parameters!", [.. unresolved]);
        }
    }

    public override void WriteDependencies(IDirectory dir)
    {
        if (ByteCode.IsValid())
        {
            if (ByteCodePlatform == ShaderByteCodePlatform.None)
            {
                throw new InvalidOperationException("Byte code platform not set!");
            }

            using IFile file = dir.CreateFile($"{ByteCode.Name}.{(char)ByteCodePlatform}{ByteCodeFileExtensionCharacter}u");
            ByteCode.Resource!.Write(file);
        }

        foreach (ResourceReference<ShaderParameter> parameter in Parameters)
        {
            if (!parameter.IsValid())
            {
                continue;
            }

            using IFile file = dir.CreateFile($"{parameter.Name}{ParameterFileExtension}");
            parameter.Resource!.Write(file);
            parameter.Resource!.WriteDependencies(dir);
        }
    }
}