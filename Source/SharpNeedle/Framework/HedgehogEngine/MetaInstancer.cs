﻿namespace SharpNeedle.Framework.HedgehogEngine;

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
        FormatVersion = reader.Read<uint>();

        int instanceCount = reader.Read<int>();
        int instanceSize = reader.Read<int>();

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
        writer.Write(FormatVersion);

        writer.Write(Instances.Count);
        writer.Write(InstanceSize);

        writer.Skip(12);

        writer.Write(HeaderSize);
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
            Position = reader.Read<Vector3>();
            Type = reader.Read<byte>();
            Sway = reader.Read<byte>();

            PitchAfterSway = reader.Read<byte>();
            YawAfterSway = reader.Read<byte>();

            PitchBeforeSway = reader.Read<short>();
            YawBeforeSway = reader.Read<short>();

            byte colorA = reader.Read<byte>();
            Color = new Color<byte>(reader.Read<byte>(), reader.Read<byte>(), reader.Read<byte>(), colorA);
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

            writer.Write(Color.A);
            writer.Write(Color.R);
            writer.Write(Color.G);
            writer.Write(Color.B);
        }
    }
}
