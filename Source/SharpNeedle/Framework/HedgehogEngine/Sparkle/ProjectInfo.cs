namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;
public class ProjectInfo : IBinarySerializable
{
    public string Editor { get; set; } = "";
    public ProjectType Type { get; set; }
    public int EmitterCount { get; set; }
    public int ParticleCount { get; set; }
    public DateTime ExportDate { get; set; }
    public DateTime Version { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Editor = reader.ReadStringPaddedByte();
        Type = reader.ReadStringPaddedByte() == "CEffect" ? ProjectType.Effect : ProjectType.Material;

        if (Type == ProjectType.Effect)
        {
            EmitterCount = reader.ReadInt32();
            ParticleCount = reader.ReadInt32();
        }
        ExportDate = DateTime.ParseExact(reader.ReadInt64().ToString(), "yyyyMMddHHmmss", System.Globalization.CultureInfo.InvariantCulture);
        Version = DateTime.ParseExact(reader.ReadInt32().ToString(), "yyyyMMdd", System.Globalization.CultureInfo.InvariantCulture);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte(Editor);
        writer.WriteStringPaddedByte(Type == ProjectType.Effect ? "CEffect" : "Material");

        if (Type == ProjectType.Effect)
        {
            writer.Write(EmitterCount);
            writer.Write(ParticleCount);
        }

        writer.Write(long.Parse(ExportDate.ToString("yyyyMMddHHmmss")));
        writer.Write(int.Parse(Version.ToString("yyyyMMdd")));
    }
}