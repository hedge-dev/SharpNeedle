namespace SharpNeedle.Framework.LostWorld.Animation;

public struct InterpolateInfo : IBinarySerializable
{
    public InterpolateMode EnterInterpolation;
    public InterpolateMode ExitInterpolation;
    public float Time;
    public string? From;

    public void Read(BinaryObjectReader reader)
    {
        EnterInterpolation = (InterpolateMode)reader.ReadInt32();
        ExitInterpolation = (InterpolateMode)reader.ReadInt32();
        Time = reader.ReadSingle();
        From = reader.ReadStringOffset();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32((int)EnterInterpolation);
        writer.WriteInt32((int)ExitInterpolation);
        writer.WriteSingle(Time);
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