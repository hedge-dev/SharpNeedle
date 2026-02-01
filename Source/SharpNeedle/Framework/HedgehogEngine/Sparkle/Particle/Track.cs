namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public struct Track : IBinarySerializable
{
    public int CurveType { get; set; }
    public int KeyCount { get; set; }
    public int FieldC { get; set; }
    public List<KeyFrame> KeyFrames { get; set; } = [];

    public Track()
    {
    }

    public void Read(BinaryObjectReader reader)
    {
        CurveType = reader.ReadInt32();
        KeyCount = reader.ReadInt32();
        FieldC = reader.ReadInt32();

        if (KeyCount > 0)
        {
            KeyFrames = new(KeyCount);
            for (int i = 0; i < KeyCount; i++)
            {
                KeyFrame keyFrame = new()
                {
                    Time = reader.ReadSingle(),
                    Value = reader.ReadSingle(),
                    ValueUpperBias = reader.ReadSingle(),
                    ValueLowerBias = reader.ReadSingle(),
                    SlopeL = reader.ReadSingle(),
                    SlopeR = reader.ReadSingle(),
                    SlopeLUpperBias = reader.ReadSingle(),
                    SlopeLLowerBias = reader.ReadSingle(),
                    SlopeRUpperBias = reader.ReadSingle(),
                    SlopeRLowerBias = reader.ReadSingle(),
                    KeyBreak = reader.ReadInt32() == 1
                };
                KeyFrames.Add(keyFrame);
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(CurveType);
        writer.WriteInt32(KeyFrames.Count);
        writer.WriteInt32(41);

        for (int i = 0; i < KeyFrames.Count; i++)
        {
            writer.WriteSingle(KeyFrames[i].Time);
            writer.WriteSingle(KeyFrames[i].Value);
            writer.WriteSingle(KeyFrames[i].ValueUpperBias);
            writer.WriteSingle(KeyFrames[i].ValueLowerBias);
            writer.WriteSingle(KeyFrames[i].SlopeL);
            writer.WriteSingle(KeyFrames[i].SlopeR);
            writer.WriteSingle(KeyFrames[i].SlopeLUpperBias);
            writer.WriteSingle(KeyFrames[i].SlopeLLowerBias);
            writer.WriteSingle(KeyFrames[i].SlopeRUpperBias);
            writer.WriteSingle(KeyFrames[i].SlopeRLowerBias);
            writer.WriteInt32(KeyFrames[i].KeyBreak ? 1 : 0);
        }
    }
}
