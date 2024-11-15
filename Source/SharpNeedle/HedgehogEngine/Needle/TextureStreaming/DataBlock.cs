using System.IO;

namespace SharpNeedle.HedgehogEngine.Needle.TextureStreaming;

public class DataBlock : IBinarySerializable
{
    public long Position { get; set; }

    public long Length { get; set; }

    public MemoryStream DataStream { get; private set; }


    public void Read(BinaryObjectReader reader)
    {
        Position = reader.ReadInt64();
        Length = reader.ReadInt64();
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


    public void EnsureData(Package package, bool overrideExisting)
    {
        if(DataStream != null)
        {
            if(overrideExisting)
            {
                DataStream.Dispose();
            }
            else
            {
                return;
            }
        }

        DataStream = new MemoryStream((int)Length);

        if(package != null)
        {
            using SubStream readStream = new(package.BaseStream, Position, Length);
            DataStream.Write(readStream.ReadAllBytes().AsSpan());
        }

        Position = 0;
    }
}
