namespace SharpNeedle.HedgehogEngine.Mirage;

[BinaryResource("hh/material", ResourceType.Material, @"\.material$")]
public class Material : SampleChunkResource
{
    public string ShaderName { get; set; }
    public byte AlphaThreshold { get; set; }
    public bool NoBackFaceCulling { get; set; }
    public bool UseAdditiveBlending { get; set; }

    public Dictionary<string, Parameter<Vector4>> FloatParameters { get; set; } = new();
    public Dictionary<string, Parameter<Vector4Int>> IntParameters { get; set; } = new();
    public Dictionary<string, Parameter<bool>> BoolParameters { get; set; } = new();
    public Texset Texset { get; set; } = new();

    public Material()
    {
        DataVersion = 3;
    }

    public override void Read(BinaryObjectReader reader)
    {
        if (Root != null)
            DataVersion = 3;

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
        UseAdditiveBlending = reader.Read<bool>();
        reader.Skip(1); // Alignment padding

        var floatParamsCount = reader.Read<byte>();
        var intParamsCount = reader.Read<byte>();
        var boolParamsCount = reader.Read<byte>();
        var textureCount = reader.Read<byte>(); // Reserved in version 1

        var floatParamOffset = reader.ReadOffsetValue();
        var intParamOffset = reader.ReadOffsetValue();
        var boolParamOffset = reader.ReadOffsetValue();

        ReadParameters(FloatParameters, floatParamOffset, floatParamsCount);
        ReadParameters(IntParameters, intParamOffset, intParamsCount);
        ReadParameters(BoolParameters, boolParamOffset, boolParamsCount);
        
        if (DataVersion >= 2)
        {
            using var token = reader.AtOffset(textureNamesOffset);
            for (byte i = 0; i < textureCount; i++)
            {
                Texset.Textures.Add(new Texture 
                {
                    Name = reader.ReadStringOffset()
                });
            }
        }
        
        if (DataVersion <= 2)
        {
            using var token = reader.AtOffset(texsetNameOffset);
            Texset.Name = reader.ReadString(StringBinaryFormat.NullTerminated);
        }

        if (DataVersion >= 3)
        {
            using var token = reader.AtOffset(texturesOffset);

            foreach (var texture in Texset.Textures)
            {
                reader.ReadOffset(() => 
                { 
                    texture.Read(reader);
                });
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void ReadParameters<T>(Dictionary<string, Parameter<T>> paramsOut, long offset, int count) where T : unmanaged
        {
            if (offset == 0 || count == 0)
                return;

            using var token = reader.AtOffset(offset);
            for (byte i = 0; i < count; i++)
            {
                var param = reader.ReadObjectOffset<Parameter<T>>();
                paramsOut.TryAdd(param.Name, param);
            }
        }
    }

    public override void Write(BinaryObjectWriter reader)
    {
        throw new NotImplementedException();
    }

    public override void ResolveDependencies(IResourceResolver resolver)
    {
        if (DataVersion <= 1)
        {
            Texset = resolver.Open<Texset>($"{Texset.Name}.texset");
        }

        if (DataVersion <= 2)
            Texset.ResolveDependencies(resolver);
    }

    public class Parameter<T> : IBinarySerializable where T : unmanaged
    {
        internal string Name { get; set; }
        public List<T> Values { get; set; } = new(1);

        public T Value
        {
            get
            {
                if (Values.Count == 0 || Values == null)
                    return default;

                return Values[0];
            }
            set
            {
                Values ??= new List<T>(1);

                if (Values.Count == 0)
                    Values.Add(value);
                else
                    Values[0] = value;
            }
        }

        public void Read(BinaryObjectReader reader)
        {
            reader.Skip(2);
            var valueCount = reader.Read<byte>();
            reader.Skip(1);

            Name = reader.ReadStringOffset(StringBinaryFormat.NullTerminated);
            if (valueCount != 0)
            {
                using var token = reader.ReadOffset();
                for (byte i = 0; i < valueCount; i++)
                {
                    // Handle bools
                    if (typeof(T) == typeof(bool))
                        ((IList)Values).Add(reader.Read<int>() != 0);
                    else 
                        Values.Add(reader.Read<T>());
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
            writer.WriteCollectionOffset(Values, 4);
        }
    }
}