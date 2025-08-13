namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ShaderData;

using System.Diagnostics;

[NeedleResource("hh/shaderparam", @"\.[vpc]sparam")]
public class ShaderParameter : SampleChunkResource
{
    public List<ShaderConstantUsage>[] UsageSet { get; } = new List<ShaderConstantUsage>[(int)ShaderConstantType.Count];

    public ShaderParameter()
    {
        for (int i = 0; i < UsageSet.Length; i++)
        {
            UsageSet[i] = [];
        }
    }

    public IEnumerable<ShaderConstantUsage> AllUsages()
    {
        foreach (List<ShaderConstantUsage> set in UsageSet)
        {
            foreach (ShaderConstantUsage usage in set)
            {
                yield return usage;
            }
        }
    }

    public List<ShaderConstantUsage> GetUsages(ShaderConstantType type)
    {
        return UsageSet[(int)type];
    }

    public void AddUsage(ShaderConstantType type, ShaderConstantUsage usage)
    {
        usage.Type = type;
        UsageSet[(int)type].Add(usage);
    }

    public void AddUsage(ShaderConstantType type, string name, byte index, byte size = 1)
    {
        AddUsage(type, new ShaderConstantUsage { Name = name, Index = index, Size = size });
    }


    public override void Read(BinaryObjectReader reader)
    {
        for (int i = 0; i < UsageSet.Length; i++)
        {
            UsageSet[i] = reader.ReadObject<BinaryList<BinaryPointer<ShaderConstantUsage>>>().Unwind();
            foreach (ShaderConstantUsage usage in UsageSet[i])
            {
                usage.Type = (ShaderConstantType)i;
            }
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        Debug.Assert(UsageSet.Length == (int)ShaderConstantType.Count);

        foreach (List<ShaderConstantUsage> usages in UsageSet)
        {
            writer.Write(usages.Count);
            ;
            writer.WriteOffset(() =>
            {
                foreach (ShaderConstantUsage c in usages)
                {
                    writer.WriteObjectOffset(c);
                }
            });
        }
    }

    protected override void Reset()
    {
        for (int i = 0; i < UsageSet.Length; i++)
        {
            UsageSet[i] = [];
        }
    }
}
