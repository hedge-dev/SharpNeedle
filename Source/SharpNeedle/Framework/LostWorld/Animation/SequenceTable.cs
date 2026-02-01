namespace SharpNeedle.Framework.LostWorld.Animation;

public class SequenceTable : List<string?>, IComplexData
{
    public PlayModeInfo PlayMode { get; set; }

    public void Read(BinaryObjectReader reader, bool readType)
    {
        if (readType)
        {
            reader.EnsureSignatureNative(0);
        }

        Clear();
        PlayMode = reader.ReadObject<PlayModeInfo>();
        int seqCount = reader.ReadInt32();
        if (seqCount == 0)
        {
            return;
        }

        reader.ReadOffset(() =>
        {
            for (int i = 0; i < seqCount; i++)
            {
                Add(reader.ReadStringOffset());
            }
        });
    }

    public void Write(BinaryObjectWriter writer, bool writeType)
    {
        if (writeType)
        {
            writer.WriteInt32(0);
        }

        writer.WriteObject(PlayMode);
        writer.WriteInt32(Count);
        writer.WriteOffset(() =>
        {
            foreach (string? seq in this)
            {
                writer.WriteStringOffset(StringBinaryFormat.NullTerminated, seq);
            }
        });
    }
}