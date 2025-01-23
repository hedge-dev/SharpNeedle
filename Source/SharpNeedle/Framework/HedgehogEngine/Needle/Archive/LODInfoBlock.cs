namespace SharpNeedle.Framework.HedgehogEngine.Needle.Archive;

using Amicitia.IO.Binary;
using System;

public class LODInfoBlock : NeedleArchiveBlock
{
    public const string LODInfoSignature = "NEDLDI";

    public override string Signature => LODInfoSignature;

    public struct LODItem
    {
        public int CascadeFlag { get; set; }

        public float Unknown2 { get; set; }

        public byte CascadeLevel { get; set; }
    }

    public byte Unknown1 { get; set; }

    public List<LODItem> Items { get; set; } = [];


    public LODInfoBlock() : base("lodinfo") 
    {
        Version = 1;
    }


    protected override bool IsVersionValid(int version)
    {
        return version == 1;
    }

    protected override void ReadBlockData(BinaryObjectReader reader, string filename, NeedleArchvieDataOffsetMode offsetMode)
    {
        Unknown1 = reader.ReadByte();
        byte lodCount = reader.ReadByte();
        reader.Align(4);

        LODItem[] items = new LODItem[32];

        for(int i = 0; i < items.Length; i++)
        {
            items[i].CascadeFlag = reader.ReadInt32();
        }

        for(int i = 0; i < items.Length; i++)
        {
            items[i].Unknown2 = reader.ReadSingle();
        }

        for(int i = 0; i < items.Length; i++)
        {
            items[i].CascadeLevel = reader.ReadByte();
        }

        Items = [.. items[..lodCount]];
    }

    protected override void WriteBlockData(BinaryObjectWriter writer, string filename, NeedleArchvieDataOffsetMode offsetMode)
    {
        if(Items.Count > 32)
        {
            throw new InvalidOperationException($"LOD Info has too many items! Must be <= 32, is {Items.Count}");
        }

        writer.WriteByte(Unknown1);
        writer.WriteByte((byte)Items.Count);
        writer.Align(4);

        for(int i = 0; i < Items.Count; i++)
        {
            writer.WriteInt32(Items[i].CascadeFlag);
        }

        writer.WriteNulls(4 * (32 - Items.Count));

        for(int i = 0; i < Items.Count; i++)
        {
            writer.WriteSingle(Items[i].Unknown2);
        }

        writer.WriteNulls(4 * (32 - Items.Count));

        for(int i = 0; i < Items.Count; i++)
        {
            writer.WriteByte(Items[i].CascadeLevel);
        }

        writer.WriteNulls(32 - Items.Count);
    }
}
