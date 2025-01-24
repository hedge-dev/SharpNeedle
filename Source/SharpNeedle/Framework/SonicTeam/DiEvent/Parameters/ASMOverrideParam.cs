namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class ASMOverrideParam : BaseParam
{
    public string OverriddenASMName { get; set; } = string.Empty;
    public string OverridingASMName { get; set; } = string.Empty;

    public ASMOverrideParam() { }

    public ASMOverrideParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        OverriddenASMName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        OverridingASMName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteString(StringBinaryFormat.FixedLength, OverriddenASMName, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, OverridingASMName, 64);
    }

    public override int GetTypeID(GameType game)
    {
        switch(game)
        {
            case GameType.Frontiers:
                return (int)FrontiersParams.ASMForcedOverwrite;

            default:
                return 0;
        }
    }
}