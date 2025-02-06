namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

using SharpNeedle.Structs;
using SharpNeedle.Utilities;
using System.Text.Json.Serialization;

public enum MaterialBlendMode : byte
{
    Normal,
    Additive,
    Multiply,
    Subtract
}

[NeedleResource("hh/material", ResourceType.Material, @"\.material$")]
public class Material : SampleChunkResource
{

    public string? ShaderName { get; set; }
    public byte AlphaThreshold { get; set; }
    public bool NoBackFaceCulling { get; set; }
    public MaterialBlendMode BlendMode { get; set; }

    public Dictionary<string, Parameter<Vector4>> FloatParameters { get; set; } = [];
    public Dictionary<string, Parameter<Vector4Int>> IntParameters { get; set; } = [];
    public Dictionary<string, Parameter<bool>> BoolParameters { get; set; } = [];
    public Texset Texset { get; set; } = new();

    public Material()
    {
        DataVersion = 3;
    }

    public override void Read(BinaryObjectReader reader)
    {
        if (Root != null)
        {
            DataVersion = 3;
        }

        ShaderName = reader.ReadStringOffset();
        reader.Skip(4); // Unused vertex shader name (always the same as ShaderName)

        long texsetNameOffset = 0;
        long textureNamesOffset = 0;
        long texturesOffset = 0;

        if (DataVersion <= 1)
        {
            texsetNameOffset = reader.ReadOffsetValue();
            reader.Skip(4); // Reserved
        }
        else if (DataVersion == 2)
        {
            texsetNameOffset = reader.ReadOffsetValue();
            textureNamesOffset = reader.ReadOffsetValue();
        }
        else if (DataVersion >= 3)
        {
            textureNamesOffset = reader.ReadOffsetValue();
            texturesOffset = reader.ReadOffsetValue();
        }

        AlphaThreshold = reader.Read<byte>();
        NoBackFaceCulling = reader.Read<bool>();
        BlendMode = (MaterialBlendMode)reader.ReadByte();
        reader.Skip(1); // Alignment padding

        byte floatParamsCount = reader.Read<byte>();
        byte intParamsCount = reader.Read<byte>();
        byte boolParamsCount = reader.Read<byte>();
        byte textureCount = reader.Read<byte>(); // Reserved in version 1

        long floatParamOffset = reader.ReadOffsetValue();
        long intParamOffset = reader.ReadOffsetValue();
        long boolParamOffset = reader.ReadOffsetValue();

        ReadParameters(FloatParameters, floatParamOffset, floatParamsCount);
        ReadParameters(IntParameters, intParamOffset, intParamsCount);
        ReadParameters(BoolParameters, boolParamOffset, boolParamsCount);

        if (DataVersion >= 2)
        {
            using SeekToken token = reader.AtOffset(textureNamesOffset);
            for (byte i = 0; i < textureCount; i++)
            {
                Texset.Textures.Add(new Texture
                {
                    Name = reader.ReadStringOffset() ?? string.Empty
                });
            }
        }

        if (DataVersion <= 2)
        {
            using SeekToken token = reader.AtOffset(texsetNameOffset);
            Texset.Name = reader.ReadString(StringBinaryFormat.NullTerminated);
        }

        if (DataVersion >= 3)
        {
            using SeekToken token = reader.AtOffset(texturesOffset);

            foreach (Texture texture in Texset.Textures)
            {
                reader.ReadOffset(() => texture.Read(reader));
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ReadParameters<T>(Dictionary<string, Parameter<T>> paramsOut, long offset, int count) where T : unmanaged
        {
            if (offset == 0 || count == 0)
            {
                return;
            }

            using SeekToken token = reader.AtOffset(offset);
            for (byte i = 0; i < count; i++)
            {
                global::SharpNeedle.Framework.HedgehogEngine.Mirage.Material.Parameter<T> param = reader.ReadObjectOffset<Parameter<T>>();
                paramsOut.TryAdd(param.Name, param);
            }
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        // Fix texset name
        if (string.IsNullOrEmpty(Texset.Name))
        {
            Texset.Name = Name;
        }

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ShaderName);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ShaderName); // Unused vertex shader name

        if (DataVersion <= 1)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Texset.Name);
            writer.Write(0); // Reserved
        }
        else if (DataVersion == 2)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Texset.Name);
            WriteTextureNames();
        }
        else if (DataVersion >= 3)
        {
            WriteTextureNames();
            writer.WriteOffset(() =>
            {
                foreach (Texture texture in Texset.Textures)
                {
                    writer.WriteObjectOffset(texture);
                }
            });
        }

        writer.Write(AlphaThreshold);
        writer.Write(NoBackFaceCulling);
        writer.Write(BlendMode);
        writer.Align(4);

        writer.Write((byte)FloatParameters.Count);
        writer.Write((byte)IntParameters.Count);
        writer.Write((byte)BoolParameters.Count);
        writer.Write((byte)Texset.Textures.Count);

        WriteParameters(FloatParameters);
        WriteParameters(IntParameters);
        WriteParameters(BoolParameters);

        void WriteTextureNames()
        {
            writer.WriteOffset(() =>
            {
                foreach (Texture texture in Texset.Textures)
                {
                    writer.WriteStringOffset(StringBinaryFormat.NullTerminated, texture.Name);
                }
            });
        }

        void WriteParameters<T>(Dictionary<string, Parameter<T>> parameters) where T : unmanaged
        {
            if (parameters.Count == 0)
            {
                writer.Write(0);
                return;
            }

            foreach (KeyValuePair<string, Parameter<T>> parameter in parameters)
            {
                parameter.Value.Name = parameter.Key;
            }

            writer.WriteOffset(() =>
            {
                foreach (KeyValuePair<string, Parameter<T>> parameter in parameters)
                {
                    writer.WriteOffset(() => writer.WriteObject(parameter.Value));
                }
            });
        }
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        if (DataVersion <= 1)
        {
            string resource = $"{Texset.Name}.texset";
            Texset = resolver.Open<Texset>(resource)
                ?? throw new ResourceResolveException("Failed resolve texset", [resource]);
        }

        if (DataVersion <= 2)
        {
            Texset.ResolveDependencies(resolver);
        }
    }

    public override void WriteDependencies(IDirectory dir)
    {
        if (DataVersion >= 3)
        {
            return;
        }

        if (DataVersion <= 1)
        {
            IFile texSetFile = dir.CreateFile($"{Texset.Name}.texset");
            Texset.Write(texSetFile);
        }

        Texset.WriteDependencies(dir);
    }

    public class Parameter<T> : IBinarySerializable where T : unmanaged
    {
        internal string Name { get; set; } = string.Empty;

        public List<T> Values { get; set; } = new(1);

        [JsonIgnore]
        public T Value
        {
            get
            {
                if (Values.Count == 0 || Values == null)
                {
                    return default;
                }

                return Values[0];
            }
            set
            {
                Values ??= new List<T>(1);

                if (Values.Count == 0)
                {
                    Values.Add(value);
                }
                else
                {
                    Values[0] = value;
                }
            }
        }

        public void Read(BinaryObjectReader reader)
        {
            reader.Skip(2);
            byte valueCount = reader.Read<byte>();
            reader.Skip(1);

            Name = reader.ReadStringOffsetOrEmpty();
            if (valueCount != 0)
            {
                using SeekToken token = reader.ReadOffset();
                for (byte i = 0; i < valueCount; i++)
                {
                    // Handle booleans
                    if (typeof(T) == typeof(bool))
                    {
                        Unsafe.As<List<bool>>(Values)!.Add(reader.Read<int>() != 0);
                    }
                    else
                    {
                        Values!.Add(reader.Read<T>());
                    }
                }
            }
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write((byte)(typeof(T) == typeof(bool) ? 0 : 2));
            writer.Write((byte)0);
            writer.Write((byte)Values.Count);
            writer.Write((byte)0);

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            if (typeof(T) != typeof(bool))
            {
                writer.WriteCollectionOffset(Values, 4);
            }
            else
            {
                List<bool> values = Unsafe.As<List<bool>>(Values);
                writer.WriteOffset(() =>
                {
                    foreach (bool value in values!)
                    {
                        writer.Write(value ? 1 : 0);
                    }
                });
            }
        }
    }
}