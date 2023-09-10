namespace SharpNeedle.SonicTeam;

using BINA;
using SharpNeedle;

[NeedleResource("lw/path2.bin", @"\.path2.bin$")]
public class SplinePath : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("HTAP");
    public List<PathObject> Paths { get; set; } = new();

    public override void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignature(Signature);
        reader.Skip(4); // Version?

        Paths = reader.ReadObject<BinaryList<PathObject>>().Items;
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Signature);
        writer.Write(0x200); // Version?

        writer.WriteObject(new BinaryList<PathObject>(Paths));
    }
}

public class PathObject : IBinarySerializable
{
    public string Name { get; set; }
    public bool Field04 { get; set; }
    public bool Field05 { get; set; }
    public float Distance { get; set; }
    public int Field48 { get; set; }
    public AABB Bounds { get; set; }
    public UnknownStruct Unknown { get; set; }
    public List<bool> KnotFlags { get; set; } = new();
    public List<float> KnotDistances { get; set; } = new();
    public List<Vector3> Knots { get; set; } = new();
    public List<Vector3> KnotTangentIns { get; set; } = new();
    public List<Vector3> KnotTangentOuts { get; set; } = new();
    public List<DoubleKnot> DoubleKnots { get; set; } = new();
    public List<UserData> UserDatas { get; set; } = new();

    public void Read(BinaryObjectReader reader)
    {
        Name = reader.ReadStringOffset();
        Field04 = reader.Read<bool>();
        Field05 = reader.Read<bool>();

        short knotCount = reader.Read<short>();

        Distance = reader.Read<float>();

        KnotFlags.AddRange(reader.ReadArrayOffset<bool>(knotCount));
        KnotDistances.AddRange(reader.ReadArrayOffset<float>(knotCount));
        Knots.AddRange(reader.ReadArrayOffset<Vector3>(knotCount));
        KnotTangentIns.AddRange(reader.ReadArrayOffset<Vector3>(knotCount));
        KnotTangentOuts.AddRange(reader.ReadArrayOffset<Vector3>(knotCount));

        DoubleKnots.AddRange(reader.ReadArrayOffset<DoubleKnot>(reader.Read<int>() / 2));

        Bounds = reader.Read<AABB>();

        UserDatas.AddRange(reader.ReadObjectArrayOffset<UserData>(reader.Read<int>()));

        Field48 = reader.Read<int>(); // Always 0?

        Unknown = reader.ReadObjectOffset<UnknownStruct>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(Field04);
        writer.Write(Field05);

        writer.Write((short)Knots.Count);

        writer.Write(Distance);

        writer.WriteArrayOffset(KnotFlags.ToArray(), 4);
        writer.WriteArrayOffset(KnotDistances.ToArray(), 4);
        writer.WriteArrayOffset(Knots.ToArray(), 4);
        writer.WriteArrayOffset(KnotTangentIns.ToArray(), 4);
        writer.WriteArrayOffset(KnotTangentOuts.ToArray(), 4);

        writer.Write(DoubleKnots.Count * 2);
        if (DoubleKnots.Count != 0)
            writer.WriteCollectionOffset(DoubleKnots);
        else
            writer.Write(0);

        writer.Write(Bounds);

        writer.Write(UserDatas.Count);
        writer.WriteObjectCollectionOffset(UserDatas);

        writer.Write(Field48);

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
            Name = reader.ReadStringOffset();

            Type = reader.Read<DataType>();
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
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);

            writer.Write(Type);
            switch (Type)
            {
                case DataType.Integer:
                case DataType.Float:
                    writer.Write(Value);
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
            public uint Field00 { get; set; }
            public float Field04 { get; set; }

            public void Read(BinaryObjectReader reader)
            {
                Field00 = reader.Read<uint>();
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
        public List<int> SubUnknown3s { get; set; } = new();

        public void Read(BinaryObjectReader reader)
        {
            Field00 = reader.Read<int>();

            SubUnknown1s.AddRange(reader.ReadObjectArrayOffset<SubUnknown1>(reader.Read<int>()));

            SubUnknown2s.AddRange(reader.ReadObjectArrayOffset<SubUnknown2>(reader.Read<int>()));

            SubUnknown3s.AddRange(reader.ReadArrayOffset<int>(reader.Read<int>()));
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Field00);

            writer.Write(SubUnknown1s.Count);
            writer.WriteObjectCollectionOffset(SubUnknown1s);

            writer.Write(SubUnknown2s.Count);
            writer.WriteObjectCollectionOffset(SubUnknown2s);

            writer.Write(SubUnknown3s.Count);
            writer.WriteCollectionOffset(SubUnknown3s);
        }
    }
}