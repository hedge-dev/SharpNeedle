namespace SharpNeedle.HedgehogEngine.Mirage;
using System.IO;

[NeedleResource("hh/shader", ResourceType.Material, @"\.vertexshader$")]
public class Shader : SampleChunkResource
{
    public ResourceReference<ResourceRaw> ByteCode { get; set; }
    public List<ResourceReference<ShaderParameter>> Parameters { get; set; }

    public override void Read(BinaryObjectReader reader)
    {
        ByteCode = reader.ReadStringOffset();
        long paramCount = reader.ReadOffsetValue();
        Parameters = new((int)paramCount);

        reader.ReadOffset(() =>
        {
            for(int i = 0; i < paramCount; i++)
            {
                Parameters.Add(reader.ReadStringOffset());
            }
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Path.GetFileNameWithoutExtension(ByteCode.Name));

        if(Parameters == null || Parameters.Count == 0)
        {
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
            return;
        }

        writer.WriteOffsetValue(Parameters.Count);
        writer.WriteOffset(() =>
        {
            foreach(ResourceReference<ShaderParameter> parameter in Parameters)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Path.GetFileNameWithoutExtension(parameter.Name));
            }
        });
    }
}