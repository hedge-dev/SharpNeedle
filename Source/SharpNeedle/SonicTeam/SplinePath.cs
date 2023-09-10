namespace SharpNeedle.SonicTeam;

using BINA;
using SharpNeedle;
using SharpNeedle.SurfRide.Draw;
using System.IO;
using System.Reflection.PortableExecutable;

[NeedleResource("st/spline", @"\.path(2?\.bin)?$")]
public class SplinePath : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("HTAP");
    public List<PathObject> Paths { get; set; } = new();

    public override void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignature(Signature);
        reader.Skip(4); // Version?

        int pathCount = reader.Read<int>();
        long pathOffset = reader.ReadOffsetValue();
        if (pathCount != 0 && pathOffset == 0)
        {
            reader.OffsetBinaryFormat = OffsetBinaryFormat.U64;
            pathOffset = reader.ReadOffsetValue();
        }

        reader.ReadAtOffset((pathOffset), () =>
        {
            for (int i = 0; i < pathCount; i++)
                Paths.Add(reader.ReadObject<PathObject>());
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Signature);
        writer.Write(0x200); // Version?

        writer.Write(Paths.Count);

        if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            writer.Align(8);

        if (Paths.Count != 0)
            writer.WriteObjectCollectionOffset(Paths);
        else
            writer.WriteOffsetValue(0);
    }
}

public class PathObject : IBinarySerializable
{
    public string Name { get; set; }
    public byte Field04 { get; set; }
    public float Distance { get; set; }
    public int Field48 { get; set; }
    public AABB Bounds { get; set; }
    public UnknownStruct Unknown { get; set; }
    public List<bool> KnotFlags { get; set; } = new(); // Is Enabled?
    public List<float> KnotDistances { get; set; } = new();
    public List<Vector3> Knots { get; set; } = new();
    public List<Vector3> KnotTangentIns { get; set; } = new(); // Guessed
    public List<Vector3> KnotTangentOuts { get; set; } = new(); // Guessed
    public List<DoubleKnot> DoubleKnots { get; set; } = new();
    public List<UserData> UserDatas { get; set; } = new();

    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset();
        Field04 = reader.Read<byte>();

        reader.Skip(1); // Padding?

        short knotCount = reader.Read<short>();

        Distance = reader.Read<float>();

        if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            reader.Align(8);

        if (knotCount != 0)
        {
            KnotFlags.AddRange(reader.ReadArrayOffset<bool>(knotCount));
            KnotDistances.AddRange(reader.ReadArrayOffset<float>(knotCount));
            Knots.AddRange(reader.ReadArrayOffset<Vector3>(knotCount));
            KnotTangentIns.AddRange(reader.ReadArrayOffset<Vector3>(knotCount));
            KnotTangentOuts.AddRange(reader.ReadArrayOffset<Vector3>(knotCount));
        }
        else
        {
            reader.Skip(reader.OffsetBinaryFormat == OffsetBinaryFormat.U32 ? 20 : 40);
        }

        int doubleKnotCount = reader.Read<int>() / 2;

        if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            reader.Align(8);

        if (doubleKnotCount != 0)
            DoubleKnots.AddRange(reader.ReadArrayOffset<DoubleKnot>(doubleKnotCount));
        else
            reader.Skip(reader.OffsetBinaryFormat == OffsetBinaryFormat.U32 ? 4 : 8);

        Bounds = reader.Read<AABB>();

        int userDataCount = reader.Read<int>();

        if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            reader.Align(8);

        UserDatas.AddRange(reader.ReadObjectArrayOffset<UserData>(userDataCount));

        Field48 = reader.Read<int>(); // Always 0?

        if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            reader.Align(8);

        Unknown = reader.ReadObjectOffset<UnknownStruct>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(Field04);
        writer.Write((byte)0);

        writer.Write((short)Knots.Count);

        writer.Write(Distance);

        if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            writer.Align(8);

        if (Knots.Count != 0)
        {
            writer.WriteArrayOffset(KnotFlags.ToArray());
            writer.WriteArrayOffset(KnotDistances.ToArray());
            writer.WriteArrayOffset(Knots.ToArray());
            writer.WriteArrayOffset(KnotTangentIns.ToArray());
            writer.WriteArrayOffset(KnotTangentOuts.ToArray());
        }
        else
        {
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
        }

        writer.Write(DoubleKnots.Count * 2);

        if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            writer.Align(8);

        if (DoubleKnots.Count != 0)
            writer.WriteCollectionOffset(DoubleKnots);
        else
            writer.WriteOffsetValue(0);

        writer.Write(Bounds);

        writer.Write(UserDatas.Count);

        if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            writer.Align(8);

        if (UserDatas.Count != 0)
            writer.WriteObjectCollectionOffset(UserDatas);
        else
            writer.WriteOffsetValue(0);

        writer.Write(Field48);

        if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            writer.Align(8);

        writer.WriteObjectOffset(Unknown);
    }

    public override string ToString()
    {
        return Name;
    }

    public struct DoubleKnot 
    {
        public Vector3 Left { get; set; }
        public Vector3 Right { get; set; }
    }

    public class UserData : IBinarySerializable
    {
        public string Name { get; set; }
        public DataType Type { get; set; }
        public uint Value { get; set; }
        public string StringValue { get; set; }

        public unsafe float FloatValue
        {
            get
            {
                var v = Value;
                return *(float*)&v;
            }

            set => Value = *(uint*)&value;
        }

        public void Read(BinaryObjectReader reader)
        {
            if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                reader.Align(8);

            Name = reader.ReadStringOffset();

            Type = reader.Read<DataType>();

            if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                reader.Align(8);

            switch (Type)
            {
                case DataType.Float:
                case DataType.Integer:
                    Value = reader.Read<uint>();
                    break;

                case DataType.String:
                    StringValue = reader.ReadStringOffset();
                    break;

                default:
                    throw new NotImplementedException($"{Type} is not recognized");
            }
        }

        public void Write(BinaryObjectWriter writer)
        {
            if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                writer.Align(8);

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

            writer.Write(Type);

            if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                writer.Align(8);

            switch (Type)
            {
                case DataType.Integer:
                case DataType.Float:
                    writer.Write(Value);
                    
                    if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                        writer.Write(0);
                    break;

                case DataType.String:
                    writer.WriteStringOffset(StringBinaryFormat.NullTerminated, StringValue);
                    break;

                default:
                    throw new NotImplementedException($"{Type} is not recognized");
            }
        }

        public override string ToString()
        {
            return Name;
        }

        public enum DataType
        {
            Integer,
            Float, // Guessed
            String
        }
    }

    public class UnknownStruct : IBinarySerializable
    {
        public struct SubUnknown1 : IBinarySerializable
        {
            public float Field00 { get; set; }
            public float Field04 { get; set; }

            public void Read(BinaryObjectReader reader)
            {
                Field00 = reader.Read<float>();
                Field04 = reader.Read<float>();
            }

            public void Write(BinaryObjectWriter writer)
            {
                writer.Write(Field00);
                writer.Write(Field04);
            }
        }

        public struct SubUnknown2 : IBinarySerializable
        {
            public int Field00 { get; set; }
            public int Field04 { get; set; }

            public void Read(BinaryObjectReader reader)
            {
                Field00 = reader.Read<int>();
                Field04 = reader.Read<int>();
            }

            public void Write(BinaryObjectWriter writer)
            {
                writer.Write(Field00);
                writer.Write(Field04);
            }
        }

        public int Field00 { get; set; }
        public List<SubUnknown1> SubUnknown1s { get; set; } = new();
        public List<SubUnknown2> SubUnknown2s { get; set; } = new();
        public List<int> SubUnknown3s { get; set; } = new(); // Indices?

        public void Read(BinaryObjectReader reader)
        {
            Field00 = reader.Read<int>();

            int subUnknown1Count = reader.Read<int>();

            if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                reader.Align(8);

            SubUnknown1s.AddRange(reader.ReadObjectArrayOffset<SubUnknown1>(subUnknown1Count));

            int subUnknown2Count = reader.Read<int>();

            if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                reader.Align(8);

            SubUnknown2s.AddRange(reader.ReadObjectArrayOffset<SubUnknown2>(subUnknown2Count));
            
            int subUnknown3Count = reader.Read<int>();

            if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                reader.Align(8);

            SubUnknown3s.AddRange(reader.ReadArrayOffset<int>(subUnknown3Count));
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Field00);

            writer.Write(SubUnknown1s.Count);

            if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                writer.Align(8);

            writer.WriteObjectCollectionOffset(SubUnknown1s);

            writer.Write(SubUnknown2s.Count);

            if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                writer.Align(8);

            writer.WriteObjectCollectionOffset(SubUnknown2s);

            writer.Write(SubUnknown3s.Count);

            if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                writer.Align(8);

            writer.WriteCollectionOffset(SubUnknown3s);
        }
    }
}