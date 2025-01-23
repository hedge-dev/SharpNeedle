namespace SharpNeedle.Framework.SonicTeam.DiEvent.NodeData;

public class CameraData : BaseNodeData
{
    public int Field00 { get; set; }
    public int FrameCount { get; set; }
    public int Field08 { get; set; }
    public int Field0C { get; set; }
    public List<float> FrameTimes { get; set; } = new List<float>();
    public List<float> FrameData { get; set; } = new List<float>();

    public CameraData() { }
    public CameraData(BinaryObjectReader reader, GameType game)
        => Read(reader, game);

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Field00 = reader.Read<int>();
        FrameCount = reader.Read<int>();
        Field08 = reader.Read<int>();
        Field0C = reader.Read<int>();

        if(FrameCount > 0)
        {
            reader.ReadCollection(FrameCount, FrameTimes);
            reader.ReadCollection(FrameCount, FrameData);
        }
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.Write(Field00);
        writer.Write(FrameCount);
        writer.Write(Field08);
        writer.Write(Field0C);

        if(FrameCount > 0)
        {
            writer.WriteCollection(FrameTimes);
            writer.WriteCollection(FrameData);
        }
    }
}