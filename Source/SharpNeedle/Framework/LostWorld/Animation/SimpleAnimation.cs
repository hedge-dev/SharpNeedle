namespace SharpNeedle.Framework.LostWorld.Animation;

public class SimpleAnimation : AnimationDef
{
    public string ResourceName { get; set; }
    public float StartFrame { get; set; }
    public float EndFrame { get; set; }
    public float Speed { get; set; }
    public PlayModeInfo PlayMode { get; set; }
    public AnimOptions Options { get; set; }
    public List<InterpolateInfo> Interpolations { get; set; }
    public List<TriggerInfo> Triggers { get; set; }

    public SimpleAnimation() : base(AnimationType.Simple)
    {

    }

    public override void Read(BinaryObjectReader reader)
    {
        base.Read(reader);
        ResourceName = reader.ReadStringOffset();
        StartFrame = reader.Read<float>();
        EndFrame = reader.Read<float>();
        Speed = reader.Read<float>();
        PlayMode = reader.Read<PlayModeInfo>();
        Options = reader.Read<AnimOptions>();
        Interpolations = reader.ReadObject<BinaryList<InterpolateInfo>>();
        Triggers = reader.ReadObject<BinaryList<TriggerInfo>>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        base.Write(writer);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ResourceName);
        writer.Write(StartFrame);
        writer.Write(EndFrame);
        writer.Write(Speed);
        writer.Write(PlayMode);
        writer.Write(Options);
        writer.WriteObject<BinaryList<InterpolateInfo>>(Interpolations);
        writer.WriteObject<BinaryList<TriggerInfo>>(Triggers);
    }
}