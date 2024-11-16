namespace SharpNeedle.HedgehogEngine.Needle.TextureStreaming;

using System.IO;

public class DataBlock : IBinarySerializable
{
    public Stream DataStream { get; set; }


    public void Read(BinaryObjectReader reader)
    {
        long position = reader.ReadInt64();
        long length = reader.ReadInt64();
        DataStream = new SubStream(reader.GetBaseStream(), position, length);
    }

    public void Write(BinaryObjectWriter writer)
    {
        if(DataStream == null)
        {
            throw new InvalidOperationException("Data block has no buffered data!");
        }

        writer.WriteOffset(WriteData, 1);
        writer.WriteInt64(DataStream.Length);
    }

    private void WriteData(BinaryObjectWriter writer)
    {
        long previousPosition = DataStream.Position;
        DataStream.Seek(0, SeekOrigin.Begin);
        writer.WriteBytes(DataStream.ReadAllBytes());
        DataStream.Seek(previousPosition, SeekOrigin.Begin);
    }


    public void EnsureData()
    {
        if(DataStream == null)
        {
            return;
        }

        byte[] data = DataStream.ReadAllBytes();
        DataStream.Dispose();
        DataStream = new MemoryStream(data);
    }
}
