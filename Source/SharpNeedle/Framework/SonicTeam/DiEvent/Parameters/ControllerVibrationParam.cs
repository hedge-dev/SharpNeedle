namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class ControllerVibrationParam : BaseParam
{
    public int Field00 { get; set; }
    public string Group { get; set; } = string.Empty;
    public string Mode { get; set; } = string.Empty;
    public uint Field84 { get; set; }
    public uint Field88 { get; set; }
    public uint Field8C { get; set; }

    public ControllerVibrationParam() { }

    public ControllerVibrationParam(BinaryObjectReader reader, GameType game)
    {
        Read(reader, game);
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        Group = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Mode = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Field84 = reader.Read<uint>();
        Field88 = reader.Read<uint>();
        Field8C = reader.Read<uint>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteString(StringBinaryFormat.FixedLength, Group, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, Mode, 64);
        writer.Write(Field84);
        writer.Write(Field88);
        writer.Write(Field8C);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.ControllerVibration; }
}