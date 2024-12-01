namespace SharpNeedle.Framework.Ninja.Csd.Motions;

using SharpNeedle.Framework.Ninja.Csd;
using SharpNeedle.Structs;

public class CastMotion : List<KeyFrameList>, IBinarySerializable
{
    public BitSet<uint> Flags { get; set; }
    public Cast Cast { get; internal set; }

    public CastMotion()
    {

    }

    public CastMotion(Cast cast)
    {
        cast.AttachMotion(this);
    }

    public void Read(BinaryObjectReader reader)
    {
        Clear();
        Flags = reader.Read<BitSet<uint>>();
        int count = Flags.PopCount();

        if(count == 0)
        {
            reader.Skip(reader.GetOffsetSize());
            return;
        }

        Capacity = count;
        reader.ReadOffset(() =>
        {
            for(int i = 0; i < Flags.BitCount; i++)
            {
                if(!Flags.Test(i))
                {
                    continue;
                }

                KeyFrameList list = reader.ReadObject<KeyFrameList>();
                list.Property = (KeyProperty)i;
                Add(list);
            }
        });
    }

    public void Write(BinaryObjectWriter writer)
    {
        if(Count == 0)
        {
            writer.Write(0);
            writer.WriteOffsetValue(0);
            return;
        }

        Flags.Reset();
        foreach(KeyFrameList list in this)
        {
            Flags.Set((int)list.Property);
        }

        writer.Write(Flags);
        writer.WriteObjectCollectionOffset(this);
    }

    public void ReadExtended(BinaryObjectReader reader)
    {
        reader.ReadOffset(() =>
        {
            foreach(KeyFrameList list in this)
            {
                list.ReadExtended(reader);
            }
        });
    }

    public void WriteExtended(BinaryObjectWriter writer)
    {
        writer.WriteOffset(() =>
        {
            if(Count == 0)
            {
                writer.WriteOffsetValue(0);
            }

            foreach(KeyFrameList list in this)
            {
                list.WriteExtended(writer);
            }
        });
    }
}