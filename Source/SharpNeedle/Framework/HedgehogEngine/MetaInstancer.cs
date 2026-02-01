namespace SharpNeedle.Framework.HedgehogEngine;

using SharpNeedle.Structs;

// Based on Skyth's HedgeGI: https://github.com/blueskythlikesclouds/HedgeGI/blob/master/Source/HedgeGI/MetaInstancer.cpp
[NeedleResource("hh/mti", @"\.mti$")]
public class MetaInstancer : ResourceBase, IBinarySerializable
{
    public static readonly uint Signature = BinaryHelper.MakeSignature<uint>("MTI ");
    public static readonly uint InstanceSize = 24U;
    public static readonly uint HeaderSize = 32U;

    public uint FormatVersion { get; set; } = 1;
    public List<Instance> Instances { get; set; } = [];

    public override void Read(IFile file)
    {
        BaseFile = file;
        Name = Path.GetFileNameWithoutExtension(file.Name);

        using BinaryObjectReader reader = new(file.Open(), StreamOwnership.Transfer, Endianness.Big);
        Read(reader);
    }

    public override void Write(IFile file)
    {
        BaseFile = file;

        using BinaryObjectWriter writer = new(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Big);
        Write(writer);
    }

    public void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignatureNative(Signature);
        FormatVersion = reader.ReadUInt32();

        int instanceCount = reader.ReadInt32();
        int instanceSize = reader.ReadInt32();

        if (instanceSize != InstanceSize)
        {
            throw new BadImageFormatException($"Instance size mismatch. Expected: {InstanceSize}. Got: {instanceSize}");
        }

        reader.Skip(12);

        reader.ReadOffset(() => Instances.AddRange(reader.ReadObjectArray<Instance>(instanceCount)));
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteNative(Signature);
        writer.WriteUInt32(FormatVersion);

        writer.WriteInt32(Instances.Count);
        writer.WriteUInt32(InstanceSize);

        writer.Skip(12);

        writer.WriteUInt32(HeaderSize);
        writer.WriteObjectCollection(Instances);
    }

    public class Instance : IBinarySerializable
    {
        public Vector3 Position { get; set; }
        public byte Type { get; set; }
        public byte Sway { get; set; }

        public byte PitchAfterSway { get; set; }
        public byte YawAfterSway { get; set; }

        public short PitchBeforeSway { get; set; }
        public short YawBeforeSway { get; set; }

        public Color<byte> Color { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            Position = reader.ReadVector3();
            Type = reader.ReadByte();
            Sway = reader.ReadByte();

            PitchAfterSway = reader.ReadByte();
            YawAfterSway = reader.ReadByte();

            PitchBeforeSway = reader.ReadInt16();
            YawBeforeSway = reader.ReadInt16();

            byte colorA = reader.ReadByte();
            Color = new Color<byte>(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), colorA);
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.WriteVector3(Position);
            writer.WriteByte(Type);
            writer.WriteByte(Sway);

            writer.WriteByte(PitchAfterSway);
            writer.WriteByte(YawAfterSway);

            writer.WriteInt32(PitchBeforeSway);
            writer.WriteInt32(YawBeforeSway);

            writer.WriteInt32(Color.A);
            writer.WriteInt32(Color.R);
            writer.WriteInt32(Color.G);
            writer.WriteInt32(Color.B);
        }
    }
}
