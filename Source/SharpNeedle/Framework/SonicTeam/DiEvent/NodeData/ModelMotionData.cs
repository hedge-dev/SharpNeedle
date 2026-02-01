namespace SharpNeedle.Framework.SonicTeam.DiEvent.NodeData;

public class ModelMotionData : BaseNodeData
{
    public int Field00 { get; set; }
    public int Field04 { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public string Field10 { get; set; } = string.Empty;
    public float Field18 { get; set; }

    public ModelMotionData() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.ReadInt32();
        Field04 = reader.ReadInt32();
        Field08 = reader.ReadInt32();
        Field0C = reader.ReadInt32();
        Field10 = reader.ReadDiString(8);
        Field18 = reader.ReadSingle();

        reader.Skip(20);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteInt32(Field00);
        writer.WriteInt32(Field04);
        writer.WriteInt32(Field08);
        writer.WriteInt32(Field0C);
        writer.WriteDiString(Field10, 8);
        writer.WriteSingle(Field18);

        writer.WriteNulls(20);
    }
}