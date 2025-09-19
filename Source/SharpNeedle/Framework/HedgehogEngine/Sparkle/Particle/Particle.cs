namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

using SharpNeedle.Structs;
public class Particle : IBinarySerializable
{
    public string? ParticleName;
    public float LifeTime;
    public float LifeTimeBias;

    public float RotationZ;
    public float RotationZBias;
    public float InitialRotationZ;
    public float InitialRotationZBias;

    public float InitialSpeed;
    public float InitialSpeedBias;
    public float ZOffset;
    public float LocusDiff;

    public int NumDivision;
    public int LocusUVType;

    public bool IsBillboard;
    public bool IsEmitterLocal;

    public int LayerType;
    public int PivotType;
    public int UVDescType;
    public int TextureIndexType;
    public int TextureIndexChangeInterval;
    public int TextureIndexChangeIntervalBias;
    public int InitialTextureIndex;
    public int DirectionType;
    public int ParticleDataFlags;

    //ARGB
    public Vector4Int Color;

    public Vector4 Gravity;
    public Vector4 ExternalForce;
    public Vector4 InitialDirection;
    public Vector4 InitialDirectionBias;
    public Vector4 InitialScale;
    public Vector4 InitialScaleBias;

    public string? MeshName;

    public Vector4 RotationXYZ;
    public Vector4 RotationXYZBias;
    public Vector4 InitialRotationXYZ;
    public Vector4 InitialRotationXYZBias;
    public Vector4 UVScrollParam;
    public Vector4 UVScrollParamAlpha;

    public string? RefEffectName;
    public int RefEffectEmitTimingType;
    public float RefEffectDelayTime;

    public float DirectionalVelocityRatio;
    public float DeflectionScale;
    public float SoftScale;
    public float VelocityOffset;
    public float UserData;

    public string? MaterialName;

    public int ukn0, ukn1, ukn2, ukn3;

    public int AnimCount;
    public Animation? ParticleAnim;

    // Binary Functions


    public void Read(BinaryObjectReader reader)
    {
        ParticleName = reader.ReadStringPaddedByte();

        LifeTime = reader.ReadSingle();
        LifeTimeBias = reader.ReadSingle();

        RotationZ = reader.ReadSingle();
        RotationZBias = reader.ReadSingle();
        InitialRotationZ = reader.ReadSingle();
        InitialRotationZBias = reader.ReadSingle();

        InitialSpeed = reader.ReadSingle();
        InitialSpeedBias = reader.ReadSingle();

        ZOffset = reader.ReadSingle();

        LocusDiff = reader.ReadSingle();
        NumDivision = reader.ReadInt32();
        LocusUVType = reader.ReadInt32();

        IsBillboard = reader.ReadUInt32() == 1;
        IsEmitterLocal = reader.ReadUInt32() == 1;

        LayerType = reader.ReadInt32();
        PivotType = reader.ReadInt32();
        UVDescType = reader.ReadInt32();
        TextureIndexType = reader.ReadInt32();
        TextureIndexChangeInterval = reader.ReadInt32();
        TextureIndexChangeIntervalBias = reader.ReadInt32();
        InitialTextureIndex = reader.ReadInt32();
        DirectionType = reader.ReadInt32();
        ParticleDataFlags = reader.ReadInt32();

        Color = reader.Read<Vector4Int>();

        Gravity = reader.Read<Vector4>();
        ExternalForce = reader.Read<Vector4>();

        InitialDirection = reader.Read<Vector4>();
        InitialDirectionBias = reader.Read<Vector4>();

        InitialScale = reader.Read<Vector4>();
        InitialScaleBias = reader.Read<Vector4>();

        MeshName = reader.ReadStringPaddedByte();

        RotationXYZ = reader.Read<Vector4>();
        RotationXYZBias = reader.Read<Vector4>();
        InitialRotationXYZ = reader.Read<Vector4>();
        InitialRotationXYZBias = reader.Read<Vector4>();

        UVScrollParam = reader.Read<Vector4>();
        UVScrollParamAlpha = reader.Read<Vector4>();

        RefEffectName = reader.ReadStringPaddedByte();
        RefEffectEmitTimingType = reader.ReadInt32();
        RefEffectDelayTime = reader.ReadSingle();

        DirectionalVelocityRatio = reader.ReadSingle();
        DeflectionScale = reader.ReadSingle();
        SoftScale = reader.ReadSingle();
        VelocityOffset = reader.ReadSingle();
        UserData = reader.ReadInt32();

        MaterialName = reader.ReadStringPaddedByte();

        ukn0 = reader.ReadInt32();
        ukn1 = reader.ReadInt32();
        ukn2 = reader.ReadInt32();
        ukn3 = reader.ReadInt32();
        AnimCount = reader.ReadInt32();
        if (AnimCount > 0)
        {
            ParticleAnim = reader.ReadObject<Animation>();
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte("ParticleChunk");
        writer.WriteStringPaddedByte(ParticleName);
        writer.Write(LifeTime);
        writer.Write(LifeTimeBias);

        writer.Write(RotationZ);
        writer.Write(RotationZBias);
        writer.Write(InitialRotationZ);
        writer.Write(InitialRotationZBias);

        writer.Write(InitialSpeed);
        writer.Write(InitialSpeedBias);

        writer.Write(ZOffset);
        writer.Write(LocusDiff);
        writer.Write(NumDivision);
        writer.Write(LocusUVType);
        writer.Write(IsBillboard ? 1 : 0);
        writer.Write(IsEmitterLocal ? 1 : 0);

        writer.Write(LayerType);
        writer.Write(PivotType);
        writer.Write(UVDescType);

        writer.Write(TextureIndexType);
        writer.Write(TextureIndexChangeInterval);
        writer.Write(TextureIndexChangeIntervalBias);
        writer.Write(InitialTextureIndex);

        writer.Write(DirectionType);
        writer.Write(ParticleDataFlags);

        writer.Write(Color);

        writer.Write(Gravity);
        writer.Write(ExternalForce);
        writer.Write(InitialDirection);
        writer.Write(InitialDirectionBias);
        writer.Write(InitialScale);
        writer.Write(InitialScaleBias);

        writer.WriteStringPaddedByte(MeshName);

        writer.Write(RotationXYZ);
        writer.Write(RotationXYZBias);
        writer.Write(InitialRotationXYZ);
        writer.Write(InitialRotationXYZBias);
        writer.Write(UVScrollParam);
        writer.Write(UVScrollParamAlpha);

        writer.WriteStringPaddedByte(RefEffectName);
        writer.Write(RefEffectEmitTimingType);
        writer.Write(RefEffectDelayTime);

        writer.Write(DirectionalVelocityRatio);
        writer.Write(DeflectionScale);
        writer.Write(SoftScale);
        writer.Write(VelocityOffset);
        writer.Write(UserData);
        writer.WriteStringPaddedByte(MaterialName);

        writer.Write(ukn0);
        writer.Write(ukn1);
        writer.Write(ukn2);
        writer.Write(ukn3);

        writer.Write(AnimCount);
        if (AnimCount > 0)
        {
            writer.WriteObject(ParticleAnim);
        }
    }
}