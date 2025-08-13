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
        Bounds = reader.Read<AABB>();

        Cells.AddRange(reader.ReadObjectArrayOffset<LightFieldCell>(reader.Read<int>()));
        Probes.AddRange(reader.ReadObjectArrayOffset<LightFieldProbe, uint>(DataVersion, reader.Read<int>()));
        Indices.AddRange(reader.ReadArrayOffset<uint>(reader.Read<int>()));
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Bounds);

        writer.Write(Cells.Count);
        writer.WriteObjectCollectionOffset(Cells);

        writer.Write(Probes.Count);
        writer.WriteObjectCollectionOffset(DataVersion, Probes);

        writer.Write(Indices.Count);
        writer.WriteCollectionOffset(Indices);
    }

    protected override void Reset()
    {
        Bounds = default;
        Cells.Clear();
        Probes.Clear();
        Indices.Clear();
    }
}

public class LightFieldCell : IBinarySerializable
{
    public CellType Type { get; set; }
    public uint Index { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        Type = (CellType)reader.Read<int>();
        Index = reader.Read<uint>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write((int)Type);
        writer.Write(Index);
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
        for(int i=0; i<8; i++)
        {
            Colors[i] = new Color<byte>(reader.Read<byte>(), reader.Read<byte>(), reader.Read<byte>(), 255);
        }

        if(version >= 1)
        {
            Shadow = reader.Read<byte>();
        }
    }

    public void Write(BinaryObjectWriter writer, uint version)
    {
        for (int i = 0; i < 8; i++)
        {
            writer.Write(Colors[i].R);
            writer.Write(Colors[i].G);
            writer.Write(Colors[i].B);
        }

        if(version >= 1)
        {
            writer.Write(Shadow);
        }
    }
}