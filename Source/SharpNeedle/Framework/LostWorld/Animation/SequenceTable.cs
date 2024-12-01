namespace SharpNeedle.Framework.LostWorld.Animation;

public class SequenceTable : List<string>, IComplexData
{
    public PlayModeInfo PlayMode { get; set; }

    public void Read(BinaryObjectReader reader, bool readType)
    {
        if(readType)
        {
            reader.EnsureSignatureNative(0);
        }

        Clear();
        PlayMode = reader.Read<PlayModeInfo>();
        int seqCount = reader.Read<int>();
        if(seqCount == 0)
        {
            return;
        }

        reader.ReadOffset(() =>
        {
            for(int i = 0; i < seqCount; i++)
            {
                Add(reader.ReadStringOffset());
            }
        });
    }

    public void Write(BinaryObjectWriter writer, bool writeType)
    {
        if(writeType)
        {
            writer.Write(0);
        }

        writer.Write(PlayMode);
        writer.Write(Count);
        writer.WriteOffset(() =>
        {
            foreach(string seq in this)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, seq);
            }
        });
    }
}