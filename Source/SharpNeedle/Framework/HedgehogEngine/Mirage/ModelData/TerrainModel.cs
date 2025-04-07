namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

[NeedleResource("hh/model", ResourceType.Model, @"\.terrain-model$")]
public class TerrainModel : ModelBase
{
    public TerrainModelFlags Flags { get; set; }

    public bool Instanced
    {
        get => Flags.HasFlag(TerrainModelFlags.Instanced);
        set
        {
            if (value)
            {
                Flags |= TerrainModelFlags.Instanced;
            }
            else
            {
                Flags &= ~TerrainModelFlags.Instanced;
            }
        }
    }

    public override void Read(BinaryObjectReader reader)
    {
        CommonRead(reader);
        if (DataVersion >= 5)
        {
            Name = reader.ReadStringOffsetOrEmpty();
            Flags = reader.Read<TerrainModelFlags>();
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        CommonWrite(writer);
        if (DataVersion >= 5)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.Write(Flags);
        }
    }
}