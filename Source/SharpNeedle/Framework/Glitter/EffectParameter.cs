namespace SharpNeedle.Framework.Glitter;

using System.Net;

public class EffectParameter : IBinarySerializable<Effect>
{
    public string? Name { get; set; }
    public float StartTime { get; set; }
    public float LifeTime { get; set; }
    public int PreprocessFrame { get; set; } // Guessed
    public float EffectScale { get; set; } // Guessed
    public float EmittingScale { get; set; } // Guessed
    public float Opacity { get; set; } // Guessed
    public float Field1C { get; set; }
    public int Field50 { get; set; }
    public int Field54 { get; set; }
    public int Field58 { get; set; }
    public int Field5C { get; set; }
    public int Field60 { get; set; }
    public bool Loop { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 Scale { get; set; }
    public List<AnimationParameter> Animations { get; set; } = new(14);
    public LinkedList<EmitterParameter> Emitters { get; set; } = new();

    public void Read(BinaryObjectReader reader, Effect parent)
    {
        Name = reader.ReadStringOffset();

        StartTime = reader.ReadSingle();
        LifeTime = reader.ReadSingle();

        PreprocessFrame = reader.ReadInt32();

        EffectScale = reader.ReadSingle();
        EmittingScale = reader.ReadSingle();
        Opacity = reader.ReadSingle();

        Field1C = reader.ReadSingle();

        reader.Align(16);
        Position = reader.ReadVector3();
        reader.Align(16);
        Rotation = reader.ReadVector3().ToDegrees();
        reader.Align(16);
        Scale = reader.ReadVector3();
        reader.Align(16);

        Field50 = reader.ReadInt32();
        Field54 = reader.ReadInt32();
        Field58 = reader.ReadInt32();
        Field5C = reader.ReadInt32();
        Field60 = reader.ReadInt32();

        Loop = Convert.ToBoolean(reader.ReadInt32());

        for (int i = 0; i < Animations.Capacity; i++)
        {
            Animations.Add(new());

            long animationOffset = reader.ReadOffsetValue();
            if (animationOffset != 0)
            {
                Animations[i] = reader.ReadObjectAtOffset<AnimationParameter>(animationOffset);
            }
        }

        long emitterOffset = reader.ReadOffsetValue();
        if (emitterOffset != 0)
        {
            Emitters.AddFirst(reader.ReadObjectAtOffset<EmitterParameter, EffectParameter>(emitterOffset, this));
        }

        long nextOffset = reader.ReadOffsetValue();
        if (nextOffset != 0)
        {
            parent.Parameters.AddLast(reader.ReadObjectAtOffset<EffectParameter>(nextOffset));
        }
    }

    public void Write(BinaryObjectWriter writer, Effect parent)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

        writer.WriteSingle(StartTime);
        writer.WriteSingle(LifeTime);

        writer.WriteInt32(PreprocessFrame);

        writer.WriteSingle(EffectScale);
        writer.WriteSingle(EmittingScale);
        writer.WriteSingle(Opacity);
        writer.WriteSingle(Field1C);

        writer.Align(16);
        writer.WriteVector3(Position);
        writer.Align(16);
        writer.WriteVector3(Rotation.ToRadians());
        writer.Align(16);
        writer.WriteVector3(Scale);
        writer.Align(16);

        writer.WriteInt32(Field50);
        writer.WriteInt32(Field54);
        writer.WriteInt32(Field58);
        writer.WriteInt32(Field5C);
        writer.WriteInt32(Field60);

        writer.WriteInt32(Convert.ToInt32(Loop));

        foreach (AnimationParameter animation in Animations)
        {
            if (animation.Keyframes.Count != 0)
            {
                writer.WriteObjectOffset(animation);
            }
            else
            {
                writer.WriteOffsetValue(0);
            }
        }

        if (Emitters.First != null)
        {
            writer.WriteObjectOffset(Emitters.First.Value, this, 16);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        if (parent.Parameters.Find(this)?.Next is LinkedListNode<EffectParameter> parameter)
        {
            writer.WriteObjectOffset(parameter.Value, parent, 16);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }
    }
}
