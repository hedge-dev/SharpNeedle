namespace SharpNeedle.SonicTeam.DiEvent;

public class MaterialParameterParam : BaseParam
{
    public string MaterialName { get; set; }
    public string ParamName { get; set; }
    public uint Type { get; set; }
    public uint[] UnknownData { get; set; } = new uint[40];

    public MaterialParameterParam() { }
    public MaterialParameterParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        MaterialName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        ParamName = reader.ReadString(StringBinaryFormat.FixedLength, 64);
        Type = reader.Read<uint>();
        reader.ReadArray<uint>(40, UnknownData);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteString(StringBinaryFormat.FixedLength, MaterialName, 64);
        writer.WriteString(StringBinaryFormat.FixedLength, ParamName, 64);
        writer.Write(Type);
        writer.WriteArrayFixedLength(UnknownData, 40);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.MaterialParameter; }

    public enum ParamType
    {
        Float = 3
    }
}