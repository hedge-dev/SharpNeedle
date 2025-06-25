namespace SharpNeedle.Framework.HedgehogEngine.Needle.Shader;

using Amicitia.IO.Binary;
using SharpNeedle.Framework.HedgehogEngine.Needle.Shader.Variable;

public class ShaderGlobalVariables : IBinarySerializable
{
    public bool IncludeTerminator { get; set; }

    public List<Texture> Textures { get; set; } = [];

    public List<Sampler> Samplers { get; set; } = [];

    public List<ConstantBuffer> ConstantBuffers { get; set; } = [];

    public List<ConstantBufferField> CBBooleans { get; set; } = [];

    public List<ConstantBufferField> CBIntegers { get; set; } = [];

    public List<ConstantBufferField> CBFloats { get; set; } = [];

    public List<UnorderedAccessView> UnorderedAccessViews { get; set; } = [];


    public void Read(BinaryObjectReader reader)
    {
        Textures = [];
        Samplers = [];
        ConstantBuffers = [];
        CBBooleans = [];
        CBIntegers = [];
        CBFloats = [];
        UnorderedAccessViews = [];

        long start = reader.Position;
        int size = reader.ReadInt32();
        long end = start + size;

        // In games before SXSG, the lists seem to terminate
        // on the type "9", aka UAVs. This is easily
        // checkable by comparing pos + 4 to the end

        while (reader.Position + 4 < end)
        {
            VariantVariableType type = (VariantVariableType)reader.ReadInt32();
            long variablesStart = reader.Position;
            int variablesSize = reader.ReadInt32();
            long variablesEnd = variablesStart + variablesSize;

            while (reader.Position < variablesEnd)
            {
                switch (type)
                {
                    case VariantVariableType.Boolean:
                    case VariantVariableType.Integer:
                    case VariantVariableType.Float:
                        throw new NotImplementedException("Not implemented yet (if this occured with a vanilla file, please open a github issue for it!!!)");
                    case VariantVariableType.Texture:
                        Textures.Add(reader.ReadObject<Texture>());
                        break;
                    case VariantVariableType.Sampler:
                        Samplers.Add(reader.ReadObject<Sampler>());
                        break;
                    case VariantVariableType.ConstantBuffer:
                        ConstantBuffers.Add(reader.ReadObject<ConstantBuffer>());
                        break;
                    case VariantVariableType.CBBoolean:
                        CBBooleans.Add(reader.ReadObject<ConstantBufferField>());
                        break;
                    case VariantVariableType.CBInteger:
                        CBIntegers.Add(reader.ReadObject<ConstantBufferField>());
                        break;
                    case VariantVariableType.CBFloat:
                        CBFloats.Add(reader.ReadObject<ConstantBufferField>());
                        break;
                    case VariantVariableType.UnorderedAccessView:
                        UnorderedAccessViews.Add(reader.ReadObject<UnorderedAccessView>());
                        break;
                    default:
                        throw new InvalidDataException("Unknown variable type!");
                }
            }
        }

        IncludeTerminator = reader.Position < end;

        // should at most be 4 due to the +4
        reader.Skip((int)(end - reader.Position));
    }

    public void Write(BinaryObjectWriter writer)
    {
        void WriteArray<T>(List<T> list, VariantVariableType type) where T : IBinarySerializable
        {
            if (list.Count == 0)
            {
                return;
            }

            writer.WriteInt32((int)type);

            SeekToken sizeToken = writer.At();
            long startPosition = writer.Position;
            writer.WriteInt32(0);

            writer.WriteObjectCollection(list);

            long endPosition = writer.Position;
            using (SeekToken temp = writer.At())
            {
                sizeToken.Dispose();
                writer.Write((int)(endPosition - startPosition));
            }
        }

        SeekToken sizeToken = writer.At();
        long startPosition = writer.Position;
        writer.WriteInt32(0);

        WriteArray(Textures, VariantVariableType.Texture);
        WriteArray(Samplers, VariantVariableType.Sampler);
        WriteArray(ConstantBuffers, VariantVariableType.ConstantBuffer);
        WriteArray(CBBooleans, VariantVariableType.CBBoolean);
        WriteArray(CBIntegers, VariantVariableType.CBInteger);
        WriteArray(CBFloats, VariantVariableType.CBFloat);

        if (IncludeTerminator)
        {
            writer.WriteUInt32(9);
        }
        else
        {
            WriteArray(UnorderedAccessViews, VariantVariableType.UnorderedAccessView);
        }

        long endPosition = writer.Position;
        using (SeekToken temp = writer.At())
        {
            sizeToken.Dispose();
            writer.Write((int)(endPosition - startPosition));
        }
    }
}
