namespace SharpNeedle.HedgehogEngine;
using System.IO;

// Based on Skyth's HedgeGI: https://github.com/blueskythlikesclouds/HedgeGI/blob/master/Source/HedgeGI/MetaInstancer.cpp
[NeedleResource("hh/mti", @"\.mti$")]
public class MetaInstancer : ResourceBase, IBinarySerializable
{
    public static readonly uint Signature = BinaryHelper.MakeSignature<uint>("MTI ");
    public static readonly uint InstanceSize = 24U;
    public static readonly uint HeaderSize = 32U;
    public uint FormatVersion { get; set; } = 1;
    public List<Instance> Instances { get; set; } = new List<Instance>();
    
    public override void Read(IFile file)
    {
        using var reader = new BinaryObjectReader(file.Open(), StreamOwnership.Transfer, Endianness.Big);

        Read(reader);
    }

    public override void Write(IFile file)
    {
        using var writer = new BinaryObjectWriter(file.Open(FileAccess.Write), StreamOwnership.Transfer, Endianness.Big);

        Write(writer);
    }

    public void Read(BinaryObjectReader reader)
    {
        int sig = reader.ReadNative<int>();
        if(sig != Signature)
            throw new BadImageFormatException($"Signature mismatch. Expected: {Signature}. Got: {sig}");

        FormatVersion = reader.Read<uint>();
        int instanceCount = reader.Read<int>();
        int instanceSize = reader.Read<int>();

        if (instanceSize != InstanceSize)
            throw new BadImageFormatException($"Instance size mismatch. Expected: {InstanceSize}. Got: {instanceSize}");

        reader.Skip(12);
        
        reader.ReadOffset(() =>
        {
            Instances.AddRange(reader.ReadObjectArray<Instance>(instanceCount));
        });
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteNative(Signature);
        writer.Write(FormatVersion);
        writer.Write(Instances.Count);
        writer.Write(InstanceSize);

        writer.Skip(12);

        writer.Write(HeaderSize);
        writer.WriteObjectCollection(Instances);
    }

    public class Instance : IBinarySerializable
    {
        public Vector3 Position;
        public byte Type;
        public byte Sway;

        public byte PitchAfterSway;
        public byte YawAfterSway;

        public short PitchBeforeSway;
        public short YawBeforeSway;

        public byte ColorA;
        public byte ColorR;
        public byte ColorG;
        public byte ColorB;

        public void Read(BinaryObjectReader reader)
        {
            Position = reader.Read<Vector3>();
            Type = reader.Read<byte>();
            Sway = reader.Read<byte>();

            PitchAfterSway = reader.Read<byte>();
            YawAfterSway = reader.Read<byte>();

            PitchBeforeSway = reader.Read<short>();
            YawBeforeSway = reader.Read<short>();

            ColorA = reader.Read<byte>();
            ColorR = reader.Read<byte>();
            ColorG = reader.Read<byte>();
            ColorB = reader.Read<byte>();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Position);
            writer.Write(Type);
            writer.Write(Sway);

            writer.Write(PitchAfterSway);
            writer.Write(YawAfterSway);

            writer.Write(PitchBeforeSway);
            writer.Write(YawBeforeSway);

            writer.Write(ColorA);
            writer.Write(ColorR);
            writer.Write(ColorG);
            writer.Write(ColorB);
        }
    }
}
