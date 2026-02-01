namespace SharpNeedle.Framework.Ninja.Csd.Motions;

using SharpNeedle.Structs;

public class KeyFrame : IBinarySerializable
{
    public int Frame { get; set; }
    public Union Value { get; set; }
    public InterpolationType Interpolation { get; set; }
    public float InTangent { get; set; }
    public float OutTangent { get; set; }
    public uint Field14 { get; set; }
    public AspectRatioCorrection? Correction { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Frame = reader.ReadInt32();
        Value = reader.ReadUInt32();
        Interpolation = (InterpolationType)reader.ReadUInt32();
        InTangent = reader.ReadSingle();
        OutTangent = reader.ReadSingle();
        Field14 = reader.ReadUInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32(Frame);
        writer.WriteUInt32(Value.Uint);
        writer.WriteUInt32((uint)Interpolation);
        writer.WriteSingle(InTangent);
        writer.WriteSingle(OutTangent);
        writer.WriteUInt32(Field14);
    }

    [StructLayout(LayoutKind.Explicit, Size = 4)]
    public struct Union
    {
        [FieldOffset(0)] public Color<byte> Color;
        [FieldOffset(0)] public uint Uint;
        [FieldOffset(0)] public float Float;


        public Union(Color<byte> value) : this()
        {
            Color = value;
        }

        public Union(uint value) : this()
        {
            Uint = value;
        }

        public Union(float value) : this()
        {
            Float = value;
        }


        public void Set(Color<byte> value)
        {
            Color = value;
        }

        public void Set(uint value)
        {
            Uint = value;
        }

        public void Set(float value)
        {
            Float = value;
        }


        public static implicit operator Union(float value)
        {
            return new Union(value);
        }

        public static implicit operator Union(uint value)
        {
            return new Union(value);
        }

        public static implicit operator Union(Color<byte> value)
        {
            return new Union(value);
        }

        public static implicit operator float(Union value)
        {
            return value.Float;
        }

        public static implicit operator uint(Union value)
        {
            return value.Uint;
        }

        public static implicit operator Color<byte>(Union value)
        {
            return value.Color;
        }
    }
}