﻿namespace SharpNeedle.SonicTeam;

using BINA;
using SharpNeedle;

[NeedleResource("st/spline", @"\.path(2?\.bin)?$")]
public class SplinePath : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("HTAP");
    public uint Version { get; set; }
    public List<PathObject> Paths { get; set; } = new();

    public override void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignature(Signature);
        
        Version = reader.Read<uint>();

        int pathCount = reader.Read<int>();
        long pathOffset = reader.ReadOffsetValue();
        if (pathCount != 0 && pathOffset == 0)
        {
            reader.OffsetBinaryFormat = OffsetBinaryFormat.U64;
            pathOffset = reader.ReadOffsetValue();
        }

        Dictionary<string, uint> paths = new();
        if (Version == 1)
        {
            reader.ReadAtOffset(pathOffset, () =>
            {
                for (uint i = 0; i < pathCount; i++)
                    paths.Add(reader.ReadStringOffset(), reader.Read<uint>());
            });

            paths = paths.OrderBy(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            pathCount = reader.Read<int>();
            pathOffset = reader.ReadOffsetValue();
            if (pathCount != 0 && pathOffset == 0)
            {
                reader.OffsetBinaryFormat = OffsetBinaryFormat.U64;
                pathOffset = reader.ReadOffsetValue();
            }
        }

        reader.ReadAtOffset((pathOffset), () =>
        {
            for (int i = 0; i < pathCount; i++)
                Paths.Add(reader.ReadObject<PathObject, uint>(Version));
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Signature);
        writer.Write(Version);

        writer.Write(Paths.Count);

        if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            writer.Align(8);

        if (Version == 1)
        {
            writer.WriteOffset(() =>
            {
                for (int i = 0; i < Paths.Count; i++)
                {
                    writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Paths[i].Name);
                    writer.Write(i);
                }
            });
        }

        writer.Write(Paths.Count);

        if (Paths.Count != 0)
            writer.WriteObjectCollectionOffset(Version, Paths);
        else
            writer.WriteOffsetValue(0);

        if (Version == 1)
        {
            var guidePath = Paths.FirstOrDefault(obj => obj.Name == "StageGuidePath");

            if (guidePath != null)
            {
                writer.WriteOffset(() =>
                {
                    writer.WriteStringOffset(StringBinaryFormat.NullTerminated, guidePath.Name);
                });
            }
            else
            {
                writer.WriteOffsetValue(0);
            }

            List<List<PathObject>> paths = new() { new(), new(), new(), new(), new() };

            foreach (var path in Paths)
            {
                if (path.Name.Equals("StageGuidePath", StringComparison.InvariantCulture))
                    continue;

                if (path.Name.LastIndexOf('@') == -1)
                {
                    paths[0].Add(path);
                }
                else
                {
                    var type = path.Name.AsSpan(path.Name.LastIndexOf('@') + 1);
                    if (type.StartsWith("QS", StringComparison.InvariantCulture))
                        paths[1].Add(path);
                    else if (type.StartsWith("SV", StringComparison.InvariantCulture))
                        paths[2].Add(path);
                    else if (type.StartsWith("GR", StringComparison.InvariantCulture))
                        paths[3].Add(path);
                    else if (type.StartsWith("DP", StringComparison.InvariantCulture))
                        paths[4].Add(path);
                    else
                        throw new NotImplementedException($"{type} is not recognized");
                }
            }

            foreach (var list in paths)
            {
                writer.Write(list.Count);
                if (list.Count != 0)
                {
                    writer.WriteOffset(() =>
                    {
                        foreach (var path in list)
                            writer.WriteObjectOffset(path, Version);
                    });
                }
                else
                {
                    writer.WriteOffsetValue(0);
                }
            }
        }
    }
}

public class PathObject : IBinarySerializable<uint>
{
    public string Name { get; set; }
    public byte Field04 { get; set; }
    public float Distance { get; set; }
    public int Field48 { get; set; }
    public AABB Bounds { get; set; }
    public UnknownStruct Unknown { get; set; }
    public List<int> KnotFlags { get; set; } = new(); // Knot Type?
    public List<float> KnotDistances { get; set; } = new();
    public List<float> KnotField2Cs { get; set; } = new(); // Only used by Version 1
    public List<Vector3> Knots { get; set; } = new();
    public List<Vector3> KnotTangentIns { get; set; } = new(); // Guessed
    public List<Vector3> KnotTangentOuts { get; set; } = new(); // Guessed
    public List<DoubleKnot> DoubleKnots { get; set; } = new();
    public List<UserData> UserDatas { get; set; } = new();

    public void Read(BinaryObjectReader reader, uint version)
    {
        if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            reader.Align(8);

        Name = reader.ReadStringOffset();

        if (version == 1)
        {
            reader.Skip(4); // Always 0?
            reader.Skip(4); // Always 1?

            Distance = reader.Read<float>();

            uint knotCount = reader.Read<uint>();

            if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                reader.Align(8);

            reader.ReadOffset(() =>
            {
                for (int i = 0; i < knotCount; i++)
                {
                    KnotDistances.Add(reader.Read<float>());
                    KnotFlags.Add(reader.Read<int>());
                    Knots.Add(reader.Read<Vector3>());
                    KnotTangentIns.Add(reader.Read<Vector3>());
                    KnotTangentOuts.Add(reader.Read<Vector3>());
                    KnotField2Cs.Add(reader.Read<float>());
                }
            });

            int doubleKnotCount = reader.Read<int>() / 2;

            if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                reader.Align(8);

            if (doubleKnotCount != 0)
                DoubleKnots.AddRange(reader.ReadArrayOffset<DoubleKnot>(doubleKnotCount));
            else
                reader.Skip(reader.OffsetBinaryFormat == OffsetBinaryFormat.U32 ? 4 : 8);
        }
        else
        {
            Field04 = reader.Read<byte>();

            reader.Skip(1); // Padding?

            short knotCount = reader.Read<short>();

            Distance = reader.Read<float>();

            if (reader.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                reader.Align(8);

            if (knotCount != 0)
            {
                reader.ReadOffset(() =>
                {
                    for (int i = 0; i < knotCount; i++)
                        KnotFlags.Add(reader.Read<byte>());
                });
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
    }

    public void Write(BinaryObjectWriter writer, uint version)
    {
        if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
            writer.Align(8);

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        
        if (version == 1)
        {
            writer.Write(0); // Always 0?
            writer.Write(1); // Always 1?

            writer.Write(Distance);

            writer.Write(Knots.Count);

            if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                writer.Align(8);

            if (Knots.Count != 0)
            {
                writer.WriteOffset(() =>
                {
                    for (int i = 0; i < Knots.Count; i++)
                    {
                        writer.Write(KnotDistances[i]);
                        writer.Write(KnotFlags[i]);
                        writer.Write(Knots[i]);
                        writer.Write(KnotTangentIns[i]);
                        writer.Write(KnotTangentOuts[i]);
                        writer.Write(KnotField2Cs[i]);
                    }
                });
            }
            else
            {
                writer.WriteOffsetValue(0);
            }

            writer.Write(DoubleKnots.Count * 2);

            if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                writer.Align(8);

            if (DoubleKnots.Count != 0)
                writer.WriteCollectionOffset(DoubleKnots);
            else
                writer.WriteOffsetValue(0);
        }
        else
        {
            writer.Write(Field04);
            writer.Write((byte)0);

            writer.Write((short)Knots.Count);

            writer.Write(Distance);

            if (writer.OffsetBinaryFormat == OffsetBinaryFormat.U64)
                writer.Align(8);

            if (Knots.Count != 0)
            {
                writer.WriteOffset(() =>
                {
                    for (int i = 0; i < Knots.Count; i++)
                        writer.Write((byte)KnotFlags[i]);
                });
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
            reader.Align(reader.GetOffsetSize());

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
            writer.Align(writer.GetOffsetSize());

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

        public enum DataType : byte
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