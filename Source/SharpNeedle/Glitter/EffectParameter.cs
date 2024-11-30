namespace SharpNeedle.Glitter;

public class EffectParameter : IBinarySerializable<Effect>
{
    public string Name { get; set; }
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

        StartTime = reader.Read<float>();
        LifeTime = reader.Read<float>();

        PreprocessFrame = reader.Read<int>();

        EffectScale = reader.Read<float>();
        EmittingScale = reader.Read<float>();
        Opacity = reader.Read<float>();

        Field1C = reader.Read<float>();

        reader.Align(16);
        Position = reader.Read<Vector3>();
        reader.Align(16);
        Rotation = MathHelper.ToDegrees(reader.Read<Vector3>());
        reader.Align(16);
        Scale = reader.Read<Vector3>();
        reader.Align(16);

        Field50 = reader.Read<int>();
        Field54 = reader.Read<int>();
        Field58 = reader.Read<int>();
        Field5C = reader.Read<int>();
        Field60 = reader.Read<int>();

        Loop = Convert.ToBoolean(reader.Read<int>());

        for(int i = 0; i < Animations.Capacity; i++)
        {
            Animations.Add(new());

            long animationOffset = reader.ReadOffsetValue();
            if(animationOffset != 0)
            {
                Animations[i] = reader.ReadObjectAtOffset<AnimationParameter>(animationOffset);
            }
        }

        long emitterOffset = reader.ReadOffsetValue();
        if(emitterOffset != 0)
        {
            Emitters.AddFirst(reader.ReadObjectAtOffset<EmitterParameter, EffectParameter>(emitterOffset, this));
        }

        long nextOffset = reader.ReadOffsetValue();
        if(nextOffset != 0)
        {
            parent.Parameters.AddLast(reader.ReadObjectAtOffset<EffectParameter>(nextOffset));
        }
    }

    public void Write(BinaryObjectWriter writer, Effect parent)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

        writer.Write(StartTime);
        writer.Write(LifeTime);

        writer.Write(PreprocessFrame);

        writer.Write(EffectScale);
        writer.Write(EmittingScale);
        writer.Write(Opacity);
        writer.Write(Field1C);

        writer.Align(16);
        writer.Write(Position);
        writer.Align(16);
        writer.Write(MathHelper.ToRadians(Rotation));
        writer.Align(16);
        writer.Write(Scale);
        writer.Align(16);

        writer.Write(Field50);
        writer.Write(Field54);
        writer.Write(Field58);
        writer.Write(Field5C);
        writer.Write(Field60);

        writer.Write(Convert.ToInt32(Loop));

        foreach(AnimationParameter animation in Animations)
        {
            if(animation.Keyframes.Count != 0)
            {
                writer.WriteObjectOffset(animation);
            }
            else
            {
                writer.WriteOffsetValue(0);
            }
        }

        writer.WriteObjectOffset(Emitters.First.Value, this, 16);

        if(parent.Parameters.Find(this).Next != null)
        {
            writer.WriteObjectOffset(parent.Parameters.Find(this).Next.Value, parent, 16);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }
    }
}
