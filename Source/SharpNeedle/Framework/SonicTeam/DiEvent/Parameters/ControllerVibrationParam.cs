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

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        Group = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Mode = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Field84 = reader.ReadUInt32();
        Field88 = reader.ReadUInt32();
        Field8C = reader.ReadUInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Field00);
        writer.WriteString(StringBinaryFormat.FixedLength, Group, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, Mode, 64);
        writer.WriteUInt32(Field84);
        writer.WriteUInt32(Field88);
        writer.WriteUInt32(Field8C);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.ControllerVibration; }
}