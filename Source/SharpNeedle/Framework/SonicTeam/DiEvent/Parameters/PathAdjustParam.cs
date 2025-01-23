namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class PathAdjustParam : BaseParam
{
    public Matrix4x4 LocalTransform { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public PathAdjustParam() { }
    public PathAdjustParam(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        LocalTransform = reader.Read<Matrix4x4>();
        Field40 = reader.Read<int>();
        Field44 = reader.Read<int>();
        Field48 = reader.Read<int>();
        Field4C = reader.Read<int>();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(LocalTransform);
        writer.Write(Field40);
        writer.Write(Field44);
        writer.Write(Field48);
        writer.Write(Field4C);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.PathAdjust; }
}