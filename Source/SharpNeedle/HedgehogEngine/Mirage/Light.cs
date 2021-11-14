namespace SharpNeedle.HedgehogEngine.Mirage;
using System.Diagnostics.CodeAnalysis;
using System.IO;

[BinaryResource("hh/light", ResourceType.Light)]
public class Light : SampleChunkResource
{
    public LightType Type { get; set; }
    public int Attribute { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Color { get; set; }
    public Vector4 Range { get; set; }

    public override void Read(BinaryObjectReader reader)
    {
        Type = reader.Read<LightType>();
        Position = reader.Read<Vector3>();
        Color = reader.Read<Vector3>();

        if (Type != LightType.Point)
            return;

        Attribute = reader.Read<int>();
        Range = reader.Read<Vector4>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Type);
        writer.Write(Position);
        writer.Write(Color);

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

    [ResourceCheckFunction]
    public static bool IsValid([NotNull] IFile file)
    {
        return Path.GetExtension(file.Name).Equals(".light", StringComparison.InvariantCultureIgnoreCase);
    }
}

public enum LightType : int
{
    Directional = 0,
    Point = 1
}