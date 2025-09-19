namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public class ProjectInfo : IBinarySerializable
{
    public string Editor { get; set; } = "";
    public string Type { get; set; } = "CEffect";
    public int EmitterCount { get; set; }
    public int ParticleCount { get; set; }
    public long ExportDate { get; set; }
    public int Version { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Editor = reader.ReadStringPaddedByte();
        Type = reader.ReadStringPaddedByte();

        if (Type == "CEffect")
        {
            EmitterCount = reader.ReadInt32();
            ParticleCount = reader.ReadInt32();
        }
        ExportDate = reader.ReadInt64();
        Version = reader.ReadInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte(Editor);
        writer.WriteStringPaddedByte(Type);

        if (Type == "CEffect")
        {
            writer.Write(EmitterCount);
            writer.Write(ParticleCount);
        }

        writer.Write(ExportDate);
        writer.Write(Version);
    }
}