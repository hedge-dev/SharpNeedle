namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class CullDisabledParam : BaseParam
{
    public CullDisabledParam() { }

    public override void Read(BinaryObjectReader reader, GameType game) { }

    public override void Write(BinaryObjectWriter writer, GameType game) { }

    public override int GetTypeID(GameType game) { return (int)ParameterType.CullDisabled; }
}
