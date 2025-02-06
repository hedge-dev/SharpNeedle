namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

[NeedleResource("hh/shader", ResourceType.Material, @"\.vertexshader$")]
public class Shader : SampleChunkResource
{
    public ResourceReference<ResourceRaw>? ByteCode { get; set; }
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
        if (ByteCode == null)
        {
            throw new InvalidOperationException("ByteCode is null");
        }

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Path.GetFileNameWithoutExtension(ByteCode.Value.Name));

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
}