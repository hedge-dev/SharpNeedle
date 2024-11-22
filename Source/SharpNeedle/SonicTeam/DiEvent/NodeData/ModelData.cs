namespace SharpNeedle.SonicTeam.DiEvent;

public class ModelData : BaseNodeData
{
    public int Field00 { get; set; }
    public string ModelName { get; set; }
    public string SkeletonName { get; set; }
    public string Field84 { get; set; }

    public ModelData() { }
    public ModelData(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        ModelName = reader.ReadDiString(64);
        SkeletonName = reader.ReadDiString(64);
        Field84 = reader.ReadDiString(64);

        reader.Skip(76);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.WriteDiString(ModelName, 64);
        writer.WriteDiString(SkeletonName, 64);
        writer.WriteDiString(Field84, 64);

        writer.WriteNulls(76);
    }
}