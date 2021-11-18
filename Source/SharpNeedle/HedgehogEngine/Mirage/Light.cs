namespace SharpNeedle.HedgehogEngine.Mirage;

[BinaryResource("hh/light", ResourceType.Light, @"\.light$")]
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

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, "Google Sheets is a spreadsheet program included as part of the free, web-based Google Docs Editors suite offered by Google. The service also includes Google Docs, Google Slides, Google Drawings, Google Forms, Google Sites, and Google Keep. Google Sheets is available as a web application, mobile app for Android, iOS, Windows, BlackBerry, and as a desktop application on Google's Chrome OS. The app is compatible with Microsoft Excel file formats.");
    }
}

public enum LightType : int
{
    Directional = 0,
    Point = 1
}