namespace SharpNeedle.HedgehogEngine.Mirage;
using System.Diagnostics;

public class ShaderParameter : SampleChunkResource
{
    public List<ShaderConstantUsage>[] UsageSet { get; } = new List<ShaderConstantUsage>[(int)ShaderConstantType.Count];

    public ShaderParameter()
    {
        for (var i = 0; i < UsageSet.Length; i++)
        {
            UsageSet[i] = new();
        }
    }

    public IEnumerable<ShaderConstantUsage> AllUsages()
    {
        foreach (var set in UsageSet)
        {
            foreach (var usage in set)
            {
                yield return usage;
            }
        }
    }

    public List<ShaderConstantUsage> Usage(ShaderConstantType type)
        => UsageSet[(int)type];

    public void AddUsage(ShaderConstantType type, ShaderConstantUsage usage)
    {
        usage.Type = type;
        UsageSet[(int)type].Add(usage);
    }

    public void AddUsage(ShaderConstantType type, string name, byte index, byte size = 1)
    {
        AddUsage(type, new ShaderConstantUsage { Name = name, Index = index, Size = size });
    }

    public void AddFloat4Usage(string name, byte index, byte size = 1) => AddUsage(ShaderConstantType.Float4, name, index, size);
    public void AddInt4Usage(string name, byte index, byte size = 1) => AddUsage(ShaderConstantType.Int4, name, index, size);
    public void AddBoolUsage(string name, byte index, byte size = 1) => AddUsage(ShaderConstantType.Bool, name, index, size);
    public void AddSamplerUsage(string name, byte index, byte size = 1) => AddUsage(ShaderConstantType.Sampler, name, index, size);

    public override void Read(BinaryObjectReader reader)
    {
        for (var i = 0; i < UsageSet.Length; i++)
        {
            UsageSet[i] = reader.ReadObject<BinaryList<BinaryPointer<ShaderConstantUsage>>>().Unwind();
            foreach (var usage in UsageSet[i])
            {
                usage.Type = (ShaderConstantType)i;
            }
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        Debug.Assert(UsageSet.Length == (int)ShaderConstantType.Count);

        foreach (var usages in UsageSet)
        {
            writer.Write(usages.Count);;
            writer.WriteOffset(() =>
            {
                foreach (var c in usages)
                {
                    writer.WriteObjectOffset(c);
                }
            });
        }
    }
}

public class ShaderConstantUsage : IBinarySerializable
{
    public string Name { get; set; }
    public byte Index { get; set; }
    public byte Size { get; set; } = 1;
    public ShaderConstantType Type { get; internal set; } = ShaderConstantType.Unknown;

    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset();
        Index = reader.Read<byte>();
        Size = reader.Read<byte>();
        
        reader.Align(reader.GetOffsetSize());
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(Index);
        writer.Write(Size);

        writer.Align(writer.GetOffsetSize());
    }

    public override string ToString()
    {
        return Name;
    }
}

public enum ShaderConstantType
{
    Float4,
    Int4,
    Bool,
    Sampler,

    Count,
    Unknown
}