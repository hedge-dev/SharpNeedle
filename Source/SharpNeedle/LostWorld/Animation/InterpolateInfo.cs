namespace SharpNeedle.LostWorld.Animation;

public struct InterpolateInfo : IBinarySerializable
{
    public InterpolateMode EnterInterpolation;
    public InterpolateMode ExitInterpolation;
    public float Time;
    public string From;

    public void Read(BinaryObjectReader reader)
    {
        reader.Read(out EnterInterpolation);
        reader.Read(out ExitInterpolation);
        reader.Read(out Time);
        From = reader.ReadStringOffset();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(ref EnterInterpolation);
        writer.Write(ref ExitInterpolation);
        writer.Write(ref Time);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, From);
    }
}

public enum InterpolateMode
{
    Immediate,
    StopPlay,
    StopStop,
    FinishStop,
    PlayPlay,
    PlayStop,
    FinishPlay,
    Synchronize,
}