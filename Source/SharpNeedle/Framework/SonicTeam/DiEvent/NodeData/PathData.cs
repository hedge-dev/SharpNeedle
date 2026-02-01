namespace SharpNeedle.Framework.SonicTeam.DiEvent.NodeData;

public class PathData : BaseNodeData
{
    public Matrix4x4 Transform { get; set; }
    public int Field40 { get; set; }
    public int Field44 { get; set; }
    public int Field48 { get; set; }
    public int Field4C { get; set; }

    public PathData() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Transform = reader.ReadMatrix4x4();
        Field40 = reader.ReadInt32();
        Field44 = reader.ReadInt32();
        Field48 = reader.ReadInt32();
        Field4C = reader.ReadInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteMatrix4x4(Transform);
        writer.WriteInt32(Field40);
        writer.WriteInt32(Field44);
        writer.WriteInt32(Field48);
        writer.WriteInt32(Field4C);
    }
}
