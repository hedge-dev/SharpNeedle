namespace SharpNeedle.Framework.HedgehogEngine.Mirage.LightData;
using SharpNeedle.Framework.HedgehogEngine.Mirage;

[NeedleResource(ResourceId, ResourceType.Light, @"\.light$")]
public class Light : SampleChunkResource
{
    public const string ResourceId = "hh/light";
    public const string Extension = ".light";

    public LightType Type { get; set; }
    public int Attribute { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Color { get; set; }
    public Vector4 Range { get; set; }

    public Light()
    {
        DataVersion = 1;
    }

    public override void Read(BinaryObjectReader reader)
    {
        Type = (LightType)reader.ReadInt32();
        Position = reader.ReadVector3();
        Color = reader.ReadVector3();

        if (Type == LightType.Point)
        {
            Attribute = reader.ReadInt32();
            Range = reader.ReadVector4();
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32((int)Type);
        writer.WriteVector3(Position);
        writer.WriteVector3(Color);

        if (DataVersion == 0)
        {
            return;
        }

        switch (Type)
        {
            case LightType.Point:
            {
                writer.WriteInt32(Attribute);
                writer.WriteVector4(Range);
                break;
            }

            default:
                break;
        }
    }
}