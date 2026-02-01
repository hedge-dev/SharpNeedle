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

        ParticleType = (EParticleType)reader.ReadInt32();

        LifeTime = reader.ReadSingle();

        ZOffset = reader.ReadInt32();

        DirectionType = (EDirectionType)reader.ReadInt32();

        Field14 = reader.ReadSingle();

        InitialSpeed = reader.ReadSingle();
        InitialSpeedRandomMargin = reader.ReadSingle();
        Deceleration = reader.ReadSingle();
        DecelerationRandomMargin = reader.ReadSingle();
        FollowEmitterTranslationRatio = reader.ReadSingle();
        FollowEmitterTranslationYRatio = reader.ReadSingle();

        BaseColor = reader.ReadVector4().AsColor();
        MultiPurposeColor = reader.ReadVector4().AsColor();

        long colorTableOffset = reader.ReadOffsetValue();
        if (colorTableOffset != 0)
        {
            ColorTables.AddRange(reader.ReadObjectArrayAtOffset<Color<float>>(colorTableOffset, reader.ReadInt32()));
        }

        long colorTable2Offset = reader.ReadOffsetValue();
        if (colorTable2Offset != 0)
        {
            ColorTable2s.AddRange(reader.ReadObjectArrayAtOffset<Color<float>>(colorTable2Offset, reader.ReadInt32()));
        }

        reader.Align(16);
        InitialSize = reader.ReadVector3();
        reader.Align(16);
        InitialSizeRandomMargin = reader.ReadVector3();
        reader.Align(16);
        InitialRotation = reader.ReadVector3().ToDegrees();
        reader.Align(16);
        InitialRotationRandomMargin = reader.ReadVector3().ToDegrees();
        reader.Align(16);
        Rotation = reader.ReadVector3().ToDegrees();
        reader.Align(16);
        RotationRandomMargin = reader.ReadVector3().ToDegrees();
        reader.Align(16);
        InitialDirection = reader.ReadVector3();
        reader.Align(16);
        InitialDirectionRandomMargin = reader.ReadVector3();
        reader.Align(16);
        GravitionalAcceleration = reader.ReadVector3();
        reader.Align(16);
        ExternalAcceleration = reader.ReadVector3();
        reader.Align(16);
        ExternalAccelerationRandomMargin = reader.ReadVector3();
        reader.Align(16);

        EmitterTranslationEffectRatio = reader.ReadSingle();

        LocusInterval = reader.ReadSingle();

        Field118 = reader.ReadInt32();
        Field11C = reader.ReadInt32();
        Field120 = reader.ReadInt32();
        Field124 = reader.ReadInt32();
        Field128 = reader.ReadInt32();
        Field12C = reader.ReadInt32();
        Field130 = reader.ReadInt32();
        Field134 = reader.ReadInt32();
        Field138 = reader.ReadInt32();
        Field13C = reader.ReadInt32();

        TextureIndexType = (ETextureIndexType)reader.ReadInt32();
        TextureIndex = reader.ReadInt32();
        TextureIndexRangeStart = reader.ReadInt32();
        TextureIndexRangeEnd = reader.ReadInt32();

        Field150 = reader.ReadInt32();
        Field154 = reader.ReadInt32();
        Field158 = reader.ReadInt32();
        Field15C = reader.ReadInt32();
        Field160 = reader.ReadInt32();
        Field164 = reader.ReadSingle();
        Field168 = reader.ReadInt32();
        Field16C = reader.ReadInt32();
        Field170 = reader.ReadInt32();
        Field174 = reader.ReadInt32();
        Field178 = reader.ReadInt32();
        Field17C = reader.ReadInt32();
        Field180 = reader.ReadInt32();
        Field184 = reader.ReadInt32();
        Field188 = reader.ReadInt32();
        Field18C = reader.ReadInt32();
        Field190 = reader.ReadInt32();
        Field194 = reader.ReadInt32();
        Field198 = reader.ReadInt32();
        Field19C = reader.ReadInt32();
        Field1A0 = reader.ReadInt32();

        long materialOffset = reader.ReadOffsetValue();
        if (materialOffset != 0)
        {
            Material = reader.ReadObjectAtOffset<MaterialParameter>(materialOffset);
        }

        BlendMode = (EBlendMode)reader.ReadInt32();
        CompositeMode = (ECompositeMode)reader.ReadInt32();
        SecondaryBlendMode = (ESecondaryBlendMode)reader.ReadInt32();
        SecondaryBlend = reader.ReadInt32();
        TextureAddressMode = (ETextureAddressMode)reader.ReadInt32();

        Field1BC = (int)reader.ReadOffsetValue();
        if (ParticleType == EParticleType.Mesh && Field1BC != 0)
        {
            Mesh = reader.ReadObjectAtOffset<MeshParameter>(Field1BC);
        }

        Field1C0 = reader.ReadInt32();
        Field1C4 = reader.ReadInt32();
        Field1C8 = reader.ReadInt32();
        Field1CC = reader.ReadSingle();
        Field1D0 = reader.ReadSingle();
        Field1D4 = reader.ReadInt32();
        Field1D8 = reader.ReadSingle();
        Field1DC = reader.ReadInt32();
        Field1E0 = reader.ReadInt32();
        Field1E4 = reader.ReadInt32();
        Field1E8 = reader.ReadInt32();
        Field1EC = reader.ReadInt32();

        Flags = reader.ReadInt32();

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

        writer.WriteInt32((int)ParticleType);

        writer.WriteSingle(LifeTime);

        writer.WriteInt32(ZOffset);

        writer.WriteInt32((int)DirectionType);

        writer.WriteSingle(Field14);

        writer.WriteSingle(InitialSpeed);
        writer.WriteSingle(InitialSpeedRandomMargin);
        writer.WriteSingle(Deceleration);
        writer.WriteSingle(DecelerationRandomMargin);
        writer.WriteSingle(FollowEmitterTranslationRatio);
        writer.WriteSingle(FollowEmitterTranslationYRatio);

        writer.WriteObject(BaseColor);
        writer.WriteObject(MultiPurposeColor);

        writer.WriteObjectCollectionOffset(ColorTables);
        writer.WriteInt32(ColorTables.Count);

        writer.WriteObjectCollectionOffset(ColorTable2s);
        writer.WriteInt32(ColorTable2s.Count);

        writer.Align(16);
        writer.WriteVector3(InitialSize);
        writer.Align(16);
        writer.WriteVector3(InitialSizeRandomMargin);
        writer.Align(16);
        writer.WriteVector3(InitialRotation.ToRadians());
        writer.Align(16);
        writer.WriteVector3(InitialRotationRandomMargin.ToRadians());
        writer.Align(16);
        writer.WriteVector3(Rotation.ToRadians());
        writer.Align(16);
        writer.WriteVector3(RotationRandomMargin.ToRadians());
        writer.Align(16);
        writer.WriteVector3(InitialDirection);
        writer.Align(16);
        writer.WriteVector3(InitialDirectionRandomMargin);
        writer.Align(16);
        writer.WriteVector3(GravitionalAcceleration);
        writer.Align(16);
        writer.WriteVector3(ExternalAcceleration);
        writer.Align(16);
        writer.WriteVector3(ExternalAccelerationRandomMargin);
        writer.Align(16);

        writer.WriteSingle(EmitterTranslationEffectRatio);

        writer.WriteSingle(LocusInterval);

        writer.WriteInt32(Field118);
        writer.WriteInt32(Field11C);
        writer.WriteInt32(Field120);
        writer.WriteInt32(Field124);
        writer.WriteInt32(Field128);
        writer.WriteInt32(Field12C);
        writer.WriteInt32(Field130);
        writer.WriteInt32(Field134);
        writer.WriteInt32(Field138);
        writer.WriteInt32(Field13C);

        writer.WriteInt32((int)TextureIndexType);
        writer.WriteInt32(TextureIndex);
        writer.WriteInt32(TextureIndexRangeStart);
        writer.WriteInt32(TextureIndexRangeEnd);

        writer.WriteInt32(Field150);
        writer.WriteInt32(Field154);
        writer.WriteInt32(Field158);
        writer.WriteInt32(Field15C);
        writer.WriteInt32(Field160);
        writer.WriteSingle(Field164);
        writer.WriteInt32(Field168);
        writer.WriteInt32(Field16C);
        writer.WriteInt32(Field170);
        writer.WriteInt32(Field174);
        writer.WriteInt32(Field178);
        writer.WriteInt32(Field17C);
        writer.WriteInt32(Field180);
        writer.WriteInt32(Field184);
        writer.WriteInt32(Field188);
        writer.WriteInt32(Field18C);
        writer.WriteInt32(Field190);
        writer.WriteInt32(Field194);
        writer.WriteInt32(Field198);
        writer.WriteInt32(Field19C);
        writer.WriteInt32(Field1A0);

        if (Material != null)
        {
            writer.WriteObjectOffset(Material);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        writer.WriteInt32((int)BlendMode);
        writer.WriteInt32((int)CompositeMode);
        writer.WriteInt32((int)SecondaryBlendMode);
        writer.WriteInt32(SecondaryBlend);
        writer.WriteInt32((int)TextureAddressMode);

        if (ParticleType == EParticleType.Mesh && Mesh != null)
        {
            writer.WriteObjectOffset(Mesh);
        }
        else
        {
            writer.WriteOffsetValue(Field1BC);
        }

        writer.WriteInt32(Field1C0);
        writer.WriteInt32(Field1C4);
        writer.WriteInt32(Field1C8);
        writer.WriteSingle(Field1CC);
        writer.WriteSingle(Field1D0);
        writer.WriteInt32(Field1D4);
        writer.WriteSingle(Field1D8);
        writer.WriteInt32(Field1DC);
        writer.WriteInt32(Field1E0);
        writer.WriteInt32(Field1E4);
        writer.WriteInt32(Field1E8);
        writer.WriteInt32(Field1EC);

        writer.WriteInt32(Flags);

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
