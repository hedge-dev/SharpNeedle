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
        Type = reader.Read<LightType>();
        Position = reader.Read<Vector3>();
        Color = reader.Read<Vector3>();

        if (Type != LightType.Point)
        {
            return;
        }

        Attribute = reader.Read<int>();
        Range = reader.Read<Vector4>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Type);
        writer.Write(Position);
        writer.Write(Color);

        if (DataVersion == 0)
        {
            return;
        }

        switch (Type)
        {
            case LightType.Point:
            {
                writer.Write(Attribute);
                writer.Write(Range);
                break;
            }

            default:
                break;
        }
    }
}