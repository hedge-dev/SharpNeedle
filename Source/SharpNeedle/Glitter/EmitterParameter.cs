﻿namespace SharpNeedle.Glitter;

using static SharpNeedle.Glitter.EmitterParameter.IShapeParameter;

public class EmitterParameter : IBinarySerializable<EffectParameter>
{
    public string Name { get; set; }
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

        EmitterType = reader.Read<EEmitterType>();

        StartTime = reader.Read<float>();
        LifeTime = reader.Read<float>();

        reader.Align(16);
        InitialPosition = reader.Read<Vector3>();
        reader.Align(16);
        InitialRotation = MathHelper.ToDegrees(reader.Read<Vector3>());
        reader.Align(16);
        Rotation = MathHelper.ToDegrees(reader.Read<Vector3>());
        reader.Align(16);
        RotationRandomMargin = MathHelper.ToDegrees(reader.Read<Vector3>());
        reader.Align(16);
        InitialScale = reader.Read<Vector3>();
        reader.Align(16);

        LoopStartPosition = reader.Read<float>();
        LoopEndPosition = reader.Read<float>();

        EmitCondition = reader.Read<EEmitCondition>();
        DirectionType = reader.Read<EDirectionType>();

        EmissionInterval = reader.Read<float>();
        ParticlePerEmission = reader.Read<float>();

        Field78 = reader.Read<int>();
        Field7C = reader.Read<int>();

        ShapeParameter = reader.Read<ShapeParameterUnion>();

        Field94 = reader.Read<int>();
        Field98 = reader.Read<int>();
        Field9C = reader.Read<int>();
        Field100 = reader.Read<int>();
        Field104 = reader.Read<int>();

        for (int i = 0; i < Animations.Capacity; i++)
        {
            Animations.Add(new());

            long animationOffset = reader.ReadOffsetValue();
            if (animationOffset != 0)
                Animations[i] = reader.ReadObjectAtOffset<AnimationParameter>(animationOffset);
        }

        long particleOffset = reader.ReadOffsetValue();
        if (particleOffset != 0)
            Particles.AddFirst(reader.ReadObjectAtOffset<ParticleParameter, EmitterParameter>(particleOffset, this));

        long nextOffset = reader.ReadOffsetValue();
        if (nextOffset != 0)
            parent.Emitters.AddLast(reader.ReadObjectAtOffset<EmitterParameter>(nextOffset));
    }

    public void Write(BinaryObjectWriter writer, EffectParameter parent)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

        writer.Write(EmitterType);

        writer.Write(StartTime);
        writer.Write(LifeTime);

        writer.Align(16);
        writer.Write(InitialPosition);
        writer.Align(16);
        writer.Write(MathHelper.ToRadians(InitialRotation));
        writer.Align(16);
        writer.Write(MathHelper.ToRadians(Rotation));
        writer.Align(16);
        writer.Write(MathHelper.ToRadians(RotationRandomMargin));
        writer.Align(16);
        writer.Write(InitialScale);
        writer.Align(16);

        writer.Write(LoopStartPosition);
        writer.Write(LoopEndPosition);
        
        writer.Write(EmitCondition);
        writer.Write(DirectionType);

        writer.Write(EmissionInterval);
        writer.Write(ParticlePerEmission);

        writer.Write(Field78);
        writer.Write(Field7C);

        writer.Write(ShapeParameter);

        writer.Write(Field94);
        writer.Write(Field98);
        writer.Write(Field9C);
        writer.Write(Field100);
        writer.Write(Field104);

        foreach (var animation in Animations)
        {
            if (animation.Keyframes.Count != 0)
                writer.WriteObjectOffset(animation);
            else
                writer.WriteOffsetValue(0);
        }

        writer.WriteObjectOffset(Particles.First.Value, this, 16);

        if (parent.Emitters.Find(this).Next != null)
            writer.WriteObjectOffset(parent.Emitters.Find(this).Next.Value, parent, 16);
        else
            writer.WriteOffsetValue(0);
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
    public struct ShapeParameterUnion
    {
        [FieldOffset(0)] public BoxParameter Box;
        [FieldOffset(0)] public SphereParameter Sphere;
        [FieldOffset(0)] public CylinderParameter Cylinder;
        [FieldOffset(0)] public MeshParameter Mesh;
        [FieldOffset(0)] public PolygonParameter Polygon;

        public ShapeParameterUnion(BoxParameter value) : this() => Box = value;
        public ShapeParameterUnion(SphereParameter value) : this() => Sphere = value;
        public ShapeParameterUnion(CylinderParameter value) : this() => Cylinder = value;
        public ShapeParameterUnion(MeshParameter value) : this() => Mesh = value;
        public ShapeParameterUnion(PolygonParameter value) : this() => Polygon = value;

        public void Set(BoxParameter value) => Box = value;
        public void Set(SphereParameter value) => Sphere = value;
        public void Set(CylinderParameter value) => Cylinder = value;
        public void Set(MeshParameter value) => Mesh = value;
        public void Set(PolygonParameter value) => Polygon = value;

        public static implicit operator ShapeParameterUnion(BoxParameter value) => new(value);
        public static implicit operator ShapeParameterUnion(SphereParameter value) => new(value);
        public static implicit operator ShapeParameterUnion(CylinderParameter value) => new(value);
        public static implicit operator ShapeParameterUnion(MeshParameter value) => new(value);
        public static implicit operator ShapeParameterUnion(PolygonParameter value) => new(value);

        public static implicit operator BoxParameter(ShapeParameterUnion value) => value.Box;
        public static implicit operator SphereParameter(ShapeParameterUnion value) => value.Sphere;
        public static implicit operator CylinderParameter(ShapeParameterUnion value) => value.Cylinder;
        public static implicit operator MeshParameter(ShapeParameterUnion value) => value.Mesh;
        public static implicit operator PolygonParameter(ShapeParameterUnion value) => value.Polygon;
    }
}