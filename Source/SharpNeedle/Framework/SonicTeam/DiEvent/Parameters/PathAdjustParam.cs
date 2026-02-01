namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class PathAdjustParam : BaseParam
{
    public Matrix4x4 LocalTransform { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public PathAdjustParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        LocalTransform = reader.ReadMatrix4x4();
        Field40 = reader.ReadInt32();
        Field44 = reader.ReadInt32();
        Field48 = reader.ReadInt32();
        Field4C = reader.ReadInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteMatrix4x4(LocalTransform);
        writer.WriteInt32(Field40);
        writer.WriteInt32(Field44);
        writer.WriteInt32(Field48);
        writer.WriteInt32(Field4C);
    }

    public override int GetTypeID(GameType game) { return (int)ParameterType.PathAdjust; }
}