namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

[NeedleResource("hh/model", ResourceType.Model, @"\.terrain-model$")]
public class TerrainModel : ModelBase
{
    public TerrainFlags Flags { get; set; }

    public bool Instanced
    {
        get => Flags.HasFlag(TerrainFlags.Instanced);
        set
        {
            if(value)
            {
                Flags |= TerrainFlags.Instanced;
            }
            else
            {
                Flags &= ~TerrainFlags.Instanced;
            }
        }
    }

    public override void Read(BinaryObjectReader reader)
    {
        CommonRead(reader);
        if(DataVersion >= 5)
        {
            Name = reader.ReadStringOffset();
            Flags = reader.Read<TerrainFlags>();
        }
    }

    public override void Write(BinaryObjectWriter writer)
    {
        CommonWrite(writer);
        if(DataVersion >= 5)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.Write(Flags);
        }
    }
}

[Flags]
public enum TerrainFlags : uint
{
    None,
    Instanced = 1
}