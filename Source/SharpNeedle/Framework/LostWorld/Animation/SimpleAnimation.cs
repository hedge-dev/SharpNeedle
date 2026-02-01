namespace SharpNeedle.Framework.LostWorld.Animation;

public class SimpleAnimation : AnimationDef
{
    public string? ResourceName { get; set; }
    public float StartFrame { get; set; }
    public float EndFrame { get; set; }
    public float Speed { get; set; }
    public PlayModeInfo PlayMode { get; set; }
    public AnimOptions Options { get; set; }
    public List<InterpolateInfo> Interpolations { get; set; } = [];
    public List<TriggerInfo> Triggers { get; set; } = [];

    public SimpleAnimation() : base(AnimationType.Simple)
    {

    }

    public override void Read(BinaryObjectReader reader)
    {
        base.Read(reader);
        ResourceName = reader.ReadStringOffset();
        StartFrame = reader.ReadSingle();
        EndFrame = reader.ReadSingle();
        Speed = reader.ReadSingle();
        PlayMode = reader.ReadObject<PlayModeInfo>();
        Options = (AnimOptions)reader.ReadInt32();
        Interpolations = reader.ReadObject<BinaryList<InterpolateInfo>>();
        Triggers = reader.ReadObject<BinaryList<TriggerInfo>>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        base.Write(writer);
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ResourceName);
        writer.WriteSingle(StartFrame);
        writer.WriteSingle(EndFrame);
        writer.WriteSingle(Speed);
        writer.WriteObject(PlayMode);
        writer.WriteInt32((int)Options);
        writer.WriteObject<BinaryList<InterpolateInfo>>(Interpolations);
        writer.WriteObject<BinaryList<TriggerInfo>>(Triggers);
    }
}