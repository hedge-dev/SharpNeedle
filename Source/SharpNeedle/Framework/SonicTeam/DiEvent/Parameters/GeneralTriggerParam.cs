namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class GeneralTriggerParam : BaseParam
{
    public uint Field00 { get; set; }
    public string TriggerName { get; set; } = string.Empty;

    public GeneralTriggerParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadUInt32();
        TriggerName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteUInt32(Field00);
        writer.WriteString(StringBinaryFormat.FixedLength, TriggerName, 64);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.GeneralPurposeTrigger;

            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.GeneralPurposeTrigger;

            default:
                return 0;
        }
    }
}