namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public class Emitter : IBinarySerializable
{
    public string Type;
    public int ParticleCount;
    public string EmitterName;
    public int MaxGenerateCount;
    public int GenerateCount;
    public int ParticleDataFlags;
    public bool Infinite;
    public float InitialEmittionGap;

    public Vector4 InitialPosition;
    public Vector4 RotationXYZ;
    public Vector4 RotationXYZBias;
    public Vector4 InitialRotationXYZ;
    public Vector4 InitialRotation;

    public float InitialEmitterLifeTime;
    public float EmitStartTime;
    public int EmitCondition;
    public int EmitterType;

    public Cylinder CylinderParams;
    public Sphere SphereParams;
    public Vector4 m_size;
    public string MeshName;
    public int ukn0, ukn1, ukn2, ukn3;

    public int AnimCount;
    public Animation EmitterAnim;

    public List<Particle> ParticleSaveLoad = new List<Particle>();


    
    public void Read(BinaryObjectReader reader)
    {
        // Emitter Params
        Type = reader.ReadStringPaddedByte();
        ParticleCount = reader.ReadInt32();
        EmitterName = reader.ReadStringPaddedByte();

        MaxGenerateCount = reader.ReadInt32();
        GenerateCount = reader.ReadInt32();
        ParticleDataFlags = reader.ReadInt32();
        Infinite = reader.ReadUInt32() == 1;
        InitialEmittionGap = reader.ReadSingle();

        InitialPosition = reader.Read<Vector4>();
        RotationXYZ = reader.Read<Vector4>();
        RotationXYZBias = reader.Read<Vector4>();
        InitialRotationXYZ = reader.Read<Vector4>();
        InitialRotation = reader.Read<Vector4>();

        InitialEmitterLifeTime = reader.ReadSingle();
        EmitStartTime = reader.ReadSingle();
        EmitCondition = reader.ReadInt32();
        EmitterType = reader.ReadInt32();

        CylinderParams = reader.ReadObject<Cylinder>();
        SphereParams = reader.ReadObject<Sphere>();
        m_size = new Vector4(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
        MeshName = reader.ReadStringPaddedByte();

        ukn0 = reader.ReadInt32();
        ukn1 = reader.ReadInt32();
        ukn2 = reader.ReadInt32();
        ukn3 = reader.ReadInt32();

        // Emitter Animation
        AnimCount = reader.ReadInt32();
        if (AnimCount > 0)
        {
            EmitterAnim = reader.ReadObject<Animation>();
        }

        // ParticleSaveLoadList
        if (ParticleCount > 0)
        {
            // ParticleSaveLoad
            for (int p = 0; p < ParticleCount; p++)
            {
                Particle particleSaveLoad = new Particle();

                particleSaveLoad.Type = reader.ReadStringPaddedByte();

                if (particleSaveLoad.Type == "ParticleChunk")
                {
                    ParticleSaveLoad.Add(reader.ReadObject<Particle>());
                }
            }
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte("EmitterChunk");
        writer.Write(ParticleCount);
        writer.WriteStringPaddedByte(EmitterName);
        writer.Write(MaxGenerateCount);
        writer.Write(GenerateCount);
        writer.Write(ParticleDataFlags);
        writer.Write(Infinite ? 1 : 0);
        writer.Write(InitialEmittionGap);

        writer.Write(InitialPosition);
        writer.Write(RotationXYZ);
        writer.Write(RotationXYZBias);
        writer.Write(InitialRotationXYZ);
        writer.Write(InitialRotation);

        writer.Write(InitialEmitterLifeTime);
        writer.Write(EmitStartTime);
        writer.Write(EmitCondition);
        writer.Write(EmitterType);

        writer.WriteObject(CylinderParams);
        writer.WriteObject(SphereParams);
        writer.Write(m_size);
        writer.WriteStringPaddedByte(MeshName);

        writer.Write(ukn0);
        writer.Write(ukn1);
        writer.Write(ukn2);
        writer.Write(ukn3);

        writer.Write(AnimCount);
        if (AnimCount > 0)
        {
            writer.WriteObject(EmitterAnim);
        }

        for (int i = 0; i < ParticleCount; i++)
        {
            writer.WriteObject(ParticleSaveLoad[i]);
        }
    }
}