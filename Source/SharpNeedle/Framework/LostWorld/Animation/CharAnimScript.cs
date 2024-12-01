namespace SharpNeedle.Framework.LostWorld.Animation;

using SharpNeedle.Framework.BINA;

[NeedleResource("lw/anm", @"\.anm$")]
public class CharAnimScript : BinaryResource
{
    public string ScriptVersion { get; set; } = "1.02";
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
        int triggerCount = 0;

        if(SimpleAnimations != null)
        {
            Traverse(SimpleAnimations);
        }

        if(ComplexAnimations != null)
        {
            foreach(ComplexAnimation animation in ComplexAnimations)
            {
                Traverse(animation.Animations);
            }
        }

        return triggerCount;

        void Traverse(List<SimpleAnimation> animations)
        {
            foreach(SimpleAnimation animation in animations)
            {
                if(animation.Triggers == null)
                {
                    continue;
                }

                foreach(TriggerInfo trigger in animation.Triggers)
                {
                    if(trigger.ID < triggerCount)
                    {
                        continue;
                    }

                    triggerCount = (int)trigger.ID;
                }
            }
        }
    }

    public int CountLayers()
    {
        int maxLayer = 0;

        if(SimpleAnimations != null)
        {
            Traverse(SimpleAnimations);
        }

        if(ComplexAnimations != null)
        {
            foreach(ComplexAnimation animation in ComplexAnimations)
            {
                Traverse(animation.Animations);
            }
        }

        return maxLayer + 1;

        void Traverse(List<SimpleAnimation> animations)
        {
            if(animations == null)
            {
                return;
            }

            foreach(SimpleAnimation animation in animations)
            {
                if(animation.Layer <= maxLayer)
                {
                    continue;
                }

                maxLayer = animation.Layer;
            }
        }
    }
}