namespace SharpNeedle.Framework.Glitter;

using static SharpNeedle.Framework.Glitter.EmitterParameter.IShapeParameter;

public class EmitterParameter : IBinarySerializable<EffectParameter>
{
    public string? Name { get; set; }
    public EEmitterType EmitterType { get; set; }
    public EEmitCondition EmitCondition { get; set; }
    public EDirectionType DirectionType { get; set; }
    public float StartTime { get; set; }
    public float LifeTime { get; set; }
    public float LoopStartPosition { get; set; }
    public float LoopEndPosition { get; set; }
    public float EmissionInterval { get; set; }
    public float ParticlePerEmission { get; set; }
    public int Field78 { get; set; }
    public int Field7C { get; set; }
    public int Field94 { get; set; }
    public int Field98 { get; set; }
    public int Field9C { get; set; }
    public int Field100 { get; set; }
    public int Field104 { get; set; }
    public Vector3 InitialPosition { get; set; }
    public Vector3 InitialRotation { get; set; }
    public Vector3 Rotation { get; set; }
    public Vector3 RotationRandomMargin { get; set; }
    public Vector3 InitialScale { get; set; }
    public ShapeParameterUnion ShapeParameter { get; set; }
    public List<AnimationParameter> Animations { get; set; } = new(12);
    public LinkedList<ParticleParameter> Particles { get; set; } = new();

    public void Read(BinaryObjectReader reader, EffectParameter parent)
    {
        Name = reader.ReadStringOffset();

        EmitterType = (EEmitterType)reader.ReadInt32();

        StartTime = reader.ReadSingle();
        LifeTime = reader.ReadSingle();

        reader.Align(16);
        InitialPosition = reader.ReadVector3();
        reader.Align(16);
        InitialRotation = reader.ReadVector3().ToDegrees();
        reader.Align(16);
        Rotation = reader.ReadVector3().ToDegrees();
        reader.Align(16);
        RotationRandomMargin = reader.ReadVector3().ToDegrees();
        reader.Align(16);
        InitialScale = reader.ReadVector3();
        reader.Align(16);

        LoopStartPosition = reader.ReadSingle();
        LoopEndPosition = reader.ReadSingle();

        EmitCondition = (EEmitCondition)reader.ReadInt32();
        DirectionType = (EDirectionType)reader.ReadInt32();

        EmissionInterval = reader.ReadSingle();
        ParticlePerEmission = reader.ReadSingle();

        Field78 = reader.ReadInt32();
        Field7C = reader.ReadInt32();

        ShapeParameter = reader.ReadObject<ShapeParameterUnion>();

        Field94 = reader.ReadInt32();
        Field98 = reader.ReadInt32();
        Field9C = reader.ReadInt32();
        Field100 = reader.ReadInt32();
        Field104 = reader.ReadInt32();

        for (int i = 0; i < Animations.Capacity; i++)
        {
            Animations.Add(new());

            long animationOffset = reader.ReadOffsetValue();
            if (animationOffset != 0)
            {
                Animations[i] = reader.ReadObjectAtOffset<AnimationParameter>(animationOffset);
            }
        }

        long particleOffset = reader.ReadOffsetValue();
        if (particleOffset != 0)
        {
            Particles.AddFirst(reader.ReadObjectAtOffset<ParticleParameter, EmitterParameter>(particleOffset, this));
        }

        long nextOffset = reader.ReadOffsetValue();
        if (nextOffset != 0)
        {
            parent.Emitters.AddLast(reader.ReadObjectAtOffset<EmitterParameter>(nextOffset));
        }
    }

    public void Write(BinaryObjectWriter writer, EffectParameter parent)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

        writer.WriteInt32((int)EmitterType);

        writer.WriteSingle(StartTime);
        writer.WriteSingle(LifeTime);

        writer.Align(16);
        writer.WriteVector3(InitialPosition);
        writer.Align(16);
        writer.WriteVector3(InitialRotation.ToRadians());
        writer.Align(16);
        writer.WriteVector3(Rotation.ToRadians());
        writer.Align(16);
        writer.WriteVector3(RotationRandomMargin.ToRadians());
        writer.Align(16);
        writer.WriteVector3(InitialScale);
        writer.Align(16);

        writer.WriteSingle(LoopStartPosition);
        writer.WriteSingle(LoopEndPosition);

        writer.WriteInt32((int)EmitCondition);
        writer.WriteInt32((int)DirectionType);

        writer.WriteSingle(EmissionInterval);
        writer.WriteSingle(ParticlePerEmission);

        writer.WriteInt32(Field78);
        writer.WriteInt32(Field7C);

        writer.WriteObject(ShapeParameter);

        writer.WriteInt32(Field94);
        writer.WriteInt32(Field98);
        writer.WriteInt32(Field9C);
        writer.WriteInt32(Field100);
        writer.WriteInt32(Field104);

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

        if (Particles.First != null)
        {
            writer.WriteObjectOffset(Particles.First.Value, this, 16);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        if (parent.Emitters.Find(this)?.Next is LinkedListNode<EmitterParameter> emitter)
        {
            writer.WriteObjectOffset(emitter.Value, parent, 16);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }
    }

    public enum EEmitterType : int
    {
        Cylinder = 1,
        Sphere
    };

    public enum EEmitCondition : int
    {
        Time
    };

    public enum EDirectionType : int
    {
        ParentAxis
    }

    public interface IShapeParameter
    {
        public enum EEmissionDirectionType : int
        {
            Outward = 1
        }
    }

    public struct BoxParameter : IShapeParameter
    {
        public Vector3 Size { get; set; }
    }

    public struct SphereParameter : IShapeParameter
    {
        public float Radius { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public EEmissionDirectionType EmissionDirectionType { get; set; }
    }

    public struct CylinderParameter : IShapeParameter
    {
        public float Radius { get; set; }
        public float Height { get; set; }
        public float StartAngle { get; set; }
        public float EndAngle { get; set; }
        public EEmissionDirectionType EmissionDirectionType { get; set; }
    }

    public struct MeshParameter : IShapeParameter
    {

    }

    public struct PolygonParameter : IShapeParameter
    {
        public float Radius { get; set; }
        public int PointCount { get; set; }
        public EEmissionDirectionType EmissionDirectionType { get; set; }
    }

    [StructLayout(LayoutKind.Explicit, Size = 20)]
    public struct ShapeParameterUnion : IBinarySerializable
    {
        // for reading / writing, to ensure correct endianness (for AOT)
        [FieldOffset(0)] private int _field0;
        [FieldOffset(4)] private int _field1;
        [FieldOffset(8)] private int _field2;
        [FieldOffset(12)] private int _field3;
        [FieldOffset(16)] private int _field4;

        [FieldOffset(0)] public BoxParameter Box;
        [FieldOffset(0)] public SphereParameter Sphere;
        [FieldOffset(0)] public CylinderParameter Cylinder;
        [FieldOffset(0)] public MeshParameter Mesh;
        [FieldOffset(0)] public PolygonParameter Polygon;

        public ShapeParameterUnion(BoxParameter value) : this()
        {
            Box = value;
        }

        public ShapeParameterUnion(SphereParameter value) : this()
        {
            Sphere = value;
        }

        public ShapeParameterUnion(CylinderParameter value) : this()
        {
            Cylinder = value;
        }

        public ShapeParameterUnion(MeshParameter value) : this()
        {
            Mesh = value;
        }

        public ShapeParameterUnion(PolygonParameter value) : this()
        {
            Polygon = value;
        }


        public void Read(BinaryObjectReader reader)
        {
            _field0 = reader.ReadInt32();
            _field1 = reader.ReadInt32();
            _field2 = reader.ReadInt32();
            _field3 = reader.ReadInt32();
            _field4 = reader.ReadInt32();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteInt32(_field0);
            writer.WriteInt32(_field1);
            writer.WriteInt32(_field2);
            writer.WriteInt32(_field3);
            writer.WriteInt32(_field4);
        }


        public void Set(BoxParameter value)
        {
            Box = value;
        }

        public void Set(SphereParameter value)
        {
            Sphere = value;
        }

        public void Set(CylinderParameter value)
        {
            Cylinder = value;
        }

        public void Set(MeshParameter value)
        {
            Mesh = value;
        }

        public void Set(PolygonParameter value)
        {
            Polygon = value;
        }


        public static implicit operator ShapeParameterUnion(BoxParameter value)
        {
            return new(value);
        }

        public static implicit operator ShapeParameterUnion(SphereParameter value)
        {
            return new(value);
        }

        public static implicit operator ShapeParameterUnion(CylinderParameter value)
        {
            return new(value);
        }

        public static implicit operator ShapeParameterUnion(MeshParameter value)
        {
            return new(value);
        }

        public static implicit operator ShapeParameterUnion(PolygonParameter value)
        {
            return new(value);
        }

        public static implicit operator BoxParameter(ShapeParameterUnion value)
        {
            return value.Box;
        }

        public static implicit operator SphereParameter(ShapeParameterUnion value)
        {
            return value.Sphere;
        }

        public static implicit operator CylinderParameter(ShapeParameterUnion value)
        {
            return value.Cylinder;
        }

        public static implicit operator MeshParameter(ShapeParameterUnion value)
        {
            return value.Mesh;
        }

        public static implicit operator PolygonParameter(ShapeParameterUnion value)
        {
            return value.Polygon;
        }
    }
}