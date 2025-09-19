namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public class Track : IBinarySerializable
{
    public int CurveType;
    public int KeyCount;
    public int FieldC;
    public List<KeyFrame> KeyFrames = [];

    public void Read(BinaryObjectReader reader)
    {
        CurveType = reader.ReadInt32();
        KeyCount = reader.ReadInt32();
        FieldC = reader.ReadInt32();

        if (KeyCount > 0)
        {
            KeyFrames = new List<KeyFrame>();
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
                    KeyBreak = reader.Read<int>() == 1
                };
                KeyFrames.Add(keyFrame);
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(CurveType);
        writer.Write(KeyFrames.Count);
        writer.Write(41);

        for (int i = 0; i < KeyFrames.Count; i++)
        {
            writer.Write(KeyFrames[i].Time);
            writer.Write(KeyFrames[i].Value);
            writer.Write(KeyFrames[i].ValueUpperBias);
            writer.Write(KeyFrames[i].ValueLowerBias);
            writer.Write(KeyFrames[i].SlopeL);
            writer.Write(KeyFrames[i].SlopeR);
            writer.Write(KeyFrames[i].SlopeLUpperBias);
            writer.Write(KeyFrames[i].SlopeLLowerBias);
            writer.Write(KeyFrames[i].SlopeRUpperBias);
            writer.Write(KeyFrames[i].SlopeRLowerBias);
            writer.Write<int>(KeyFrames[i].KeyBreak ? 1 : 0);
        }
    }
}
