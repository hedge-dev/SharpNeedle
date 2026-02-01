namespace SharpNeedle.Framework.HedgehogEngine.Mirage;

using SharpNeedle.Structs;

// https://github.com/blueskythlikesclouds/HedgeGI/blob/master/Source/HedgeGI/LightField.h
[NeedleResource("hh/lft", @"\.lft$")]
public class LightFieldTree : SampleChunkResource
{
    public AABB Bounds { get; set; }
    public List<LightFieldCell> Cells { get; set; } = [];
    public List<LightFieldProbe> Probes { get; set; } = [];
    public List<uint> Indices { get; set; } = [];

    public LightFieldTree()
    {
        DataVersion = 1;
    }

    public override void Read(BinaryObjectReader reader)
    {
        Bounds = reader.ReadObject<AABB>();

        Cells.AddRange(reader.ReadObjectArrayOffset<LightFieldCell>(reader.ReadInt32()));
        Probes.AddRange(reader.ReadObjectArrayOffset<LightFieldProbe, uint>(DataVersion, reader.ReadInt32()));
        Indices.AddRange(reader.ReadArrayOffset<uint>(reader.ReadInt32()));
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteObject(Bounds);

        writer.WriteInt32(Cells.Count);
        writer.WriteObjectCollectionOffset(Cells);

        writer.WriteInt32(Probes.Count);
        writer.WriteObjectCollectionOffset(DataVersion, Probes);

        writer.WriteInt32(Indices.Count);
        writer.WriteCollectionOffset(Indices);
    }
}

public class LightFieldCell : IBinarySerializable
{
    public CellType Type { get; set; }
    public uint Index { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Type = (CellType)reader.ReadInt32();
        Index = reader.ReadUInt32();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteInt32((int)Type);
        writer.WriteUInt32(Index);
    }

    public enum CellType : int
    {
        X,
        Y,
        Z,
        Probe
    }
}

public class LightFieldProbe : IBinarySerializable<uint>
{
    public Color<byte>[] Colors { get; set; } = new Color<byte>[8];
    public byte Shadow { get; set; } = 255;

    public void Read(BinaryObjectReader reader, uint version)
    {
        for (int i = 0; i < 8; i++)
        {
            Colors[i] = new Color<byte>(reader.ReadByte(), reader.ReadByte(), reader.ReadByte(), 255);
        }

        if (version >= 1)
        {
            Shadow = reader.ReadByte();
        }
    }

    public void Write(BinaryObjectWriter writer, uint version)
    {
        for (int i = 0; i < 8; i++)
        {
            writer.WriteByte(Colors[i].R);
            writer.WriteByte(Colors[i].G);
            writer.WriteByte(Colors[i].B);
        }

        if (version >= 1)
        {
            writer.WriteByte(Shadow);
        }
    }
}