namespace SharpNeedle.Framework.Glitter;

using SharpNeedle.Structs;

public class ParticleParameter : IBinarySerializable<EmitterParameter>
{
    public string? Name { get; set; }
    public EParticleType ParticleType { get; set; }
    public EDirectionType DirectionType { get; set; }
    public float LifeTime { get; set; }
    public int ZOffset { get; set; }
    public float InitialSpeed { get; set; }
    public float InitialSpeedRandomMargin { get; set; }
    public float Deceleration { get; set; }
    public float DecelerationRandomMargin { get; set; }
    public float FollowEmitterTranslationRatio { get; set; }
    public float FollowEmitterTranslationYRatio { get; set; } // Guessed
    public float EmitterTranslationEffectRatio { get; set; }
    public float LocusInterval { get; set; } // Guessed
    public ETextureIndexType TextureIndexType { get; set; }
    public int TextureIndex { get; set; }
    public int TextureIndexRangeStart { get; set; }
    public int TextureIndexRangeEnd { get; set; }
    public EBlendMode BlendMode { get; set; }
    public ECompositeMode CompositeMode { get; set; }
    public ESecondaryBlendMode SecondaryBlendMode { get; set; }
    public int SecondaryBlend { get; set; }
    public ETextureAddressMode TextureAddressMode { get; set; }
    public int Flags { get; set; }
    public float Field14 { get; set; }
    public int Field118 { get; set; }
    public int Field11C { get; set; }
    public int Field120 { get; set; }
    public int Field124 { get; set; }
    public int Field128 { get; set; }
    public int Field12C { get; set; }
    public int Field130 { get; set; }
    public int Field134 { get; set; }
    public int Field138 { get; set; }
    public int Field13C { get; set; }
    public int Field150 { get; set; }
    public int Field154 { get; set; }
    public int Field158 { get; set; }
    public int Field15C { get; set; }
    public int Field160 { get; set; }
    public float Field164 { get; set; }
    public int Field168 { get; set; }
    public int Field16C { get; set; }
    public int Field170 { get; set; }
    public int Field174 { get; set; }
    public int Field178 { get; set; }
    public int Field17C { get; set; }
    public int Field180 { get; set; }
    public int Field184 { get; set; }
    public int Field188 { get; set; }
    public int Field18C { get; set; }
    public int Field190 { get; set; }
    public int Field194 { get; set; }
    public int Field198 { get; set; }
    public int Field19C { get; set; }
    public int Field1A0 { get; set; }
    public int Field1BC { get; set; }
    public int Field1C0 { get; set; }
    public int Field1C4 { get; set; }
    public int Field1C8 { get; set; }
    public float Field1CC { get; set; }
    public float Field1D0 { get; set; }
    public int Field1D4 { get; set; }
    public float Field1D8 { get; set; }
    public int Field1DC { get; set; }
    public int Field1E0 { get; set; }
    public int Field1E4 { get; set; }
    public int Field1E8 { get; set; }
    public int Field1EC { get; set; }
    public Color<float> BaseColor { get; set; }
    public Color<float> MultiPurposeColor { get; set; }
    public Vector3 InitialSize { get; set; }
    public Vector3 InitialSizeRandomMargin { get; set; }
    public Vector3 InitialRotation { get; set; }
    public Vector3 InitialRotationRandomMargin { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 RotationRandomMargin { get; set; }
    public Vector3 InitialDirection { get; set; }
    public Vector3 InitialDirectionRandomMargin { get; set; }
    public Vector3 GravitionalAcceleration { get; set; }
    public Vector3 ExternalAcceleration { get; set; }
    public Vector3 ExternalAccelerationRandomMargin { get; set; }
    public MaterialParameter? Material { get; set; }
    public MeshParameter? Mesh { get; set; }
    public List<Color<float>> ColorTables { get; set; } = [];
    public List<Color<float>> ColorTable2s { get; set; } = [];
    public List<AnimationParameter> Animations { get; set; } = new(30);

    public void Read(BinaryObjectReader reader, EmitterParameter parent)
    {
        Name = reader.ReadStringOffset();

        ParticleType = reader.Read<EParticleType>();

        LifeTime = reader.Read<float>();

        ZOffset = reader.Read<int>();

        DirectionType = reader.Read<EDirectionType>();

        Field14 = reader.Read<float>();

        InitialSpeed = reader.Read<float>();
        InitialSpeedRandomMargin = reader.Read<float>();
        Deceleration = reader.Read<float>();
        DecelerationRandomMargin = reader.Read<float>();
        FollowEmitterTranslationRatio = reader.Read<float>();
        FollowEmitterTranslationYRatio = reader.Read<float>();

        BaseColor = reader.Read<Vector4>().AsColor();
        MultiPurposeColor = reader.Read<Vector4>().AsColor();

        long colorTableOffset = reader.ReadOffsetValue();
        if (colorTableOffset != 0)
        {
            ColorTables.AddRange(reader.ReadArrayAtOffset<Color<float>>(colorTableOffset, reader.Read<int>()));
        }

        long colorTable2Offset = reader.ReadOffsetValue();
        if (colorTable2Offset != 0)
        {
            ColorTable2s.AddRange(reader.ReadArrayAtOffset<Color<float>>(colorTable2Offset, reader.Read<int>()));
        }

        reader.Align(16);
        InitialSize = reader.Read<Vector3>();
        reader.Align(16);
        InitialSizeRandomMargin = reader.Read<Vector3>();
        reader.Align(16);
        InitialRotation = reader.Read<Vector3>().ToDegrees();
        reader.Align(16);
        InitialRotationRandomMargin = reader.Read<Vector3>().ToDegrees();
        reader.Align(16);
        Rotation = reader.Read<Vector3>().ToDegrees();
        reader.Align(16);
        RotationRandomMargin = reader.Read<Vector3>().ToDegrees();
        reader.Align(16);
        InitialDirection = reader.Read<Vector3>();
        reader.Align(16);
        InitialDirectionRandomMargin = reader.Read<Vector3>();
        reader.Align(16);
        GravitionalAcceleration = reader.Read<Vector3>();
        reader.Align(16);
        ExternalAcceleration = reader.Read<Vector3>();
        reader.Align(16);
        ExternalAccelerationRandomMargin = reader.Read<Vector3>();
        reader.Align(16);

        EmitterTranslationEffectRatio = reader.Read<float>();

        LocusInterval = reader.Read<float>();

        Field118 = reader.Read<int>();
        Field11C = reader.Read<int>();
        Field120 = reader.Read<int>();
        Field124 = reader.Read<int>();
        Field128 = reader.Read<int>();
        Field12C = reader.Read<int>();
        Field130 = reader.Read<int>();
        Field134 = reader.Read<int>();
        Field138 = reader.Read<int>();
        Field13C = reader.Read<int>();

        TextureIndexType = reader.Read<ETextureIndexType>();
        TextureIndex = reader.Read<int>();
        TextureIndexRangeStart = reader.Read<int>();
        TextureIndexRangeEnd = reader.Read<int>();

        Field150 = reader.Read<int>();
        Field154 = reader.Read<int>();
        Field158 = reader.Read<int>();
        Field15C = reader.Read<int>();
        Field160 = reader.Read<int>();
        Field164 = reader.Read<float>();
        Field168 = reader.Read<int>();
        Field16C = reader.Read<int>();
        Field170 = reader.Read<int>();
        Field174 = reader.Read<int>();
        Field178 = reader.Read<int>();
        Field17C = reader.Read<int>();
        Field180 = reader.Read<int>();
        Field184 = reader.Read<int>();
        Field188 = reader.Read<int>();
        Field18C = reader.Read<int>();
        Field190 = reader.Read<int>();
        Field194 = reader.Read<int>();
        Field198 = reader.Read<int>();
        Field19C = reader.Read<int>();
        Field1A0 = reader.Read<int>();

        long materialOffset = reader.ReadOffsetValue();
        if (materialOffset != 0)
        {
            Material = reader.ReadObjectAtOffset<MaterialParameter>(materialOffset);
        }

        BlendMode = reader.Read<EBlendMode>();
        CompositeMode = reader.Read<ECompositeMode>();
        SecondaryBlendMode = reader.Read<ESecondaryBlendMode>();
        SecondaryBlend = reader.Read<int>();
        TextureAddressMode = reader.Read<ETextureAddressMode>();

        Field1BC = (int)reader.ReadOffsetValue();
        if (ParticleType == EParticleType.Mesh && Field1BC != 0)
        {
            Mesh = reader.ReadObjectAtOffset<MeshParameter>(Field1BC);
        }

        Field1C0 = reader.Read<int>();
        Field1C4 = reader.Read<int>();
        Field1C8 = reader.Read<int>();
        Field1CC = reader.Read<float>();
        Field1D0 = reader.Read<float>();
        Field1D4 = reader.Read<int>();
        Field1D8 = reader.Read<float>();
        Field1DC = reader.Read<int>();
        Field1E0 = reader.Read<int>();
        Field1E4 = reader.Read<int>();
        Field1E8 = reader.Read<int>();
        Field1EC = reader.Read<int>();

        Flags = reader.Read<int>();

        for (int i = 0; i < Animations.Capacity; i++)
        {
            Animations.Add(new());

            long animationOffset = reader.ReadOffsetValue();
            if (animationOffset != 0)
            {
                Animations[i] = reader.ReadObjectAtOffset<AnimationParameter>(animationOffset);
            }
        }

        long nextOffset = reader.ReadOffsetValue();
        if (nextOffset != 0)
        {
            parent.Particles.AddLast(reader.ReadObjectAtOffset<ParticleParameter>(nextOffset));
        }
    }

    public void Write(BinaryObjectWriter writer, EmitterParameter parent)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

        writer.Write(ParticleType);

        writer.Write(LifeTime);

        writer.Write(ZOffset);

        writer.Write(DirectionType);

        writer.Write(Field14);

        writer.Write(InitialSpeed);
        writer.Write(InitialSpeedRandomMargin);
        writer.Write(Deceleration);
        writer.Write(DecelerationRandomMargin);
        writer.Write(FollowEmitterTranslationRatio);
        writer.Write(FollowEmitterTranslationYRatio);

        writer.Write(BaseColor);
        writer.Write(MultiPurposeColor);

        writer.WriteOffset(() =>
        {
            foreach (Color<float> colorTable in ColorTables)
            {
                writer.Write(colorTable);
            }
        });
        writer.Write(ColorTables.Count);

        writer.WriteOffset(() =>
        {
            foreach (Color<float> colorTable2 in ColorTable2s)
            {
                writer.Write(colorTable2);
            }
        });
        writer.Write(ColorTable2s.Count);

        writer.Align(16);
        writer.Write(InitialSize);
        writer.Align(16);
        writer.Write(InitialSizeRandomMargin);
        writer.Align(16);
        writer.Write(InitialRotation.ToRadians());
        writer.Align(16);
        writer.Write(InitialRotationRandomMargin.ToRadians());
        writer.Align(16);
        writer.Write(Rotation.ToRadians());
        writer.Align(16);
        writer.Write(RotationRandomMargin.ToRadians());
        writer.Align(16);
        writer.Write(InitialDirection);
        writer.Align(16);
        writer.Write(InitialDirectionRandomMargin);
        writer.Align(16);
        writer.Write(GravitionalAcceleration);
        writer.Align(16);
        writer.Write(ExternalAcceleration);
        writer.Align(16);
        writer.Write(ExternalAccelerationRandomMargin);
        writer.Align(16);

        writer.Write(EmitterTranslationEffectRatio);

        writer.Write(LocusInterval);

        writer.Write(Field118);
        writer.Write(Field11C);
        writer.Write(Field120);
        writer.Write(Field124);
        writer.Write(Field128);
        writer.Write(Field12C);
        writer.Write(Field130);
        writer.Write(Field134);
        writer.Write(Field138);
        writer.Write(Field13C);

        writer.Write(TextureIndexType);
        writer.Write(TextureIndex);
        writer.Write(TextureIndexRangeStart);
        writer.Write(TextureIndexRangeEnd);

        writer.Write(Field150);
        writer.Write(Field154);
        writer.Write(Field158);
        writer.Write(Field15C);
        writer.Write(Field160);
        writer.Write(Field164);
        writer.Write(Field168);
        writer.Write(Field16C);
        writer.Write(Field170);
        writer.Write(Field174);
        writer.Write(Field178);
        writer.Write(Field17C);
        writer.Write(Field180);
        writer.Write(Field184);
        writer.Write(Field188);
        writer.Write(Field18C);
        writer.Write(Field190);
        writer.Write(Field194);
        writer.Write(Field198);
        writer.Write(Field19C);
        writer.Write(Field1A0);

        if (Material != null)
        {
            writer.WriteObjectOffset(Material);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        writer.Write(BlendMode);
        writer.Write(CompositeMode);
        writer.Write(SecondaryBlendMode);
        writer.Write(SecondaryBlend);
        writer.Write(TextureAddressMode);

        if (ParticleType == EParticleType.Mesh && Mesh != null)
        {
            writer.WriteObjectOffset(Mesh);
        }
        else
        {
            writer.WriteOffsetValue(Field1BC);
        }

        writer.Write(Field1C0);
        writer.Write(Field1C4);
        writer.Write(Field1C8);
        writer.Write(Field1CC);
        writer.Write(Field1D0);
        writer.Write(Field1D4);
        writer.Write(Field1D8);
        writer.Write(Field1DC);
        writer.Write(Field1E0);
        writer.Write(Field1E4);
        writer.Write(Field1E8);
        writer.Write(Field1EC);

        writer.Write(Flags);

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

        if (parent.Particles.Find(this)?.Next is LinkedListNode<ParticleParameter> particle)
        {
            writer.WriteObjectOffset(particle.Value, parent, 16);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }
    }

    public enum EParticleType : int
    {
        Quad,
        Mesh,
        Locus
    }

    public enum EDirectionType : int
    {
        Billboard,
        DirectionalAngleBillboard = 2
    }

    public enum ETextureIndexType : int
    {
        Fixed,
    }

    public enum EBlendMode : int
    {
        UseMaterial = -1,
    }

    public enum ECompositeMode : int
    {
        UseMaterial = -1,
    }

    public enum ESecondaryBlendMode : int
    {
        UseMaterial = -1,
    }

    public enum ETextureAddressMode : int
    {
        UseMaterial = -1,
    }

    public class MeshParameter : IBinarySerializable
    {
        public string? Name { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            Name = reader.ReadStringOffset();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        }
    }
}
