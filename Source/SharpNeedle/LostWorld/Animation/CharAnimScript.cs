namespace SharpNeedle.LostWorld.Animation;
using BINA;

[NeedleResource("lw/anm", @"\.anm$")]
public class CharAnimScript : BinaryResource
{
    public string ScriptVersion { get; set; }
    public List<SimpleAnimation> SimpleAnimations { get; set; }
    public List<ComplexAnimation> ComplexAnimations { get; set; }
    public TransitionTable Transitions { get; set; }

    public override void Read(BinaryObjectReader reader)
    {
        BinaryHelper.EnsureSignature(reader.ReadStringOffset(), true, "ANIM");
        ScriptVersion = reader.ReadStringOffset();

        reader.Skip(4); // layerCount, triggerCount

        reader.ReadOffset(() =>
        {
            SimpleAnimations = reader.ReadObject<BinaryList<SimpleAnimation>>();
            ComplexAnimations = reader.ReadObject<BinaryList<ComplexAnimation>>();
        });

        Transitions = reader.ReadObjectOffset<TransitionTable>();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, "ANIM");
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ScriptVersion);
        writer.Write((short)CountLayers());
        writer.Write((short)CountTriggers());

        writer.WriteOffset(() =>
        {
            writer.WriteObject<BinaryList<SimpleAnimation>>(SimpleAnimations);
            writer.WriteObject<BinaryList<ComplexAnimation>>(ComplexAnimations);
        });

        writer.WriteObjectOffset(Transitions);
    }

    public int CountTriggers()
    {
        var triggerStore = new HashSet<uint>();
        
        if (SimpleAnimations != null)
            Traverse(SimpleAnimations);

        if (ComplexAnimations != null)
        {
            foreach (var animation in ComplexAnimations)
                Traverse(animation.Animations);
        }

        return triggerStore.Count;

        void Traverse(List<SimpleAnimation> animations)
        {
            foreach (var animation in animations)
            {
                if (animation.Triggers == null)
                    continue;

                foreach (var trigger in animation.Triggers)
                {
                    if (triggerStore.Contains(trigger.ID))
                        continue;

                    triggerStore.Add(trigger.ID);
                }
            }
        }
    }

    public int CountLayers()
    {
        var maxLayer = 0;
       
        if (SimpleAnimations != null)
            Traverse(SimpleAnimations);

        if (ComplexAnimations != null)
        {
            foreach (var animation in ComplexAnimations)
                Traverse(animation.Animations);
        }

        return maxLayer + 1;

        void Traverse(List<SimpleAnimation> animations)
        {
            if (animations == null)
                return;

            foreach (var animation in animations)
            {
                if (animation.Layer <= maxLayer)
                    continue;

                maxLayer = animation.Layer;
            }
        }
    }
}