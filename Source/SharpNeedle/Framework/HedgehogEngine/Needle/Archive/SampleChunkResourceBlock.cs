namespace SharpNeedle.Framework.HedgehogEngine.Needle.Archive;

using SharpNeedle.Framework.HedgehogEngine.Mirage;
using System;
using System.Linq;

public abstract class SampleChunkResourceBlock<T> : NeedleArchiveBlock where T : SampleChunkResource
{
    public T? Resource { get; set; }

    protected SampleChunkResourceBlock(string type) : base(type) { }


    private static long[] GetOffsetTable(BinaryObjectReader reader)
    {
        long startPosition = reader.Position;
        using SeekToken temp = reader.At();

        reader.Skip(8);
        long offsetsOffset = reader.ReadOffsetValue();
        int offsetsCount = reader.ReadInt32();

        reader.Seek(offsetsOffset - 0x10, SeekOrigin.Current);
        uint[] rawOffsets = reader.ReadArray<uint>(offsetsCount);
        long[] result = new long[rawOffsets.Length];

        for(int i = 0; i < rawOffsets.Length; i++)
        {
            result[i] = rawOffsets[i] + startPosition + 0x10;
        }

        return result;
    }

    private static BinaryObjectReader FlipEndianOfOffsets(BinaryObjectReader reader)
    {
        using SeekToken startToken = reader.At();

        long[] offsets = GetOffsetTable(reader);
        byte[] adjustedData = reader.GetBaseStream().ReadAllBytes();

        foreach(long offset in offsets)
        {
            (adjustedData[offset], adjustedData[offset + 3]) = (adjustedData[offset + 3], adjustedData[offset]);
            (adjustedData[offset + 1], adjustedData[offset + 2]) = (adjustedData[offset + 2], adjustedData[offset + 1]);
        }

        MemoryStream stream = new(adjustedData);
        return new(stream, StreamOwnership.Retain, reader.Endianness);
    }

    private static BinaryObjectReader FixSelfRelativeOffsets(BinaryObjectReader reader)
    {
        long startPosition = reader.Position;
        SeekToken startToken = reader.At();

        SampleChunkNode rootNode = reader.ReadObject<SampleChunkNode>();

        SampleChunkNode contexts = rootNode.FindNode("Contexts")
            ?? throw new InvalidDataException("No Contexts node!");

        SampleChunkNode? dataNode = contexts;
        while(dataNode != null && dataNode.DataSize == 0)
        {
            dataNode = dataNode.Parent;
        }

        if(dataNode == null || dataNode == rootNode)
        {
            throw new InvalidDataException("No data found!");
        }

        long dataOffsetOrigin = dataNode.DataOffset - startPosition - 0x10;

        startToken.Dispose();
        long[] offsets = GetOffsetTable(reader);
        byte[] adjustedData = reader.GetBaseStream().ReadAllBytes();

        MemoryStream stream = new(adjustedData);
        using(BinaryObjectWriter writer = new(stream, StreamOwnership.Retain, Endianness.Big))
        {
            Endianness prevEndianness = reader.Endianness;
            reader.Endianness = Endianness.Little;

            foreach(uint offset in offsets)
            {
                reader.Seek(offset, SeekOrigin.Begin);
                writer.Seek(offset, SeekOrigin.Begin);

                int relativeOffset = reader.ReadInt32();
                uint realOffset = (uint)(relativeOffset + dataOffsetOrigin);
                writer.WriteUInt32(realOffset);
            }

            reader.Endianness = prevEndianness;
        }

        stream.Position = 0;
        return new(stream, StreamOwnership.Retain, reader.Endianness);
    }


    protected override void ReadBlockData(BinaryObjectReader reader, string filename, NeedleArchvieDataOffsetMode offsetMode)
    {
        BinaryObjectReader adjustedReader = offsetMode switch
        {
            NeedleArchvieDataOffsetMode.Default => reader,
            NeedleArchvieDataOffsetMode.Flipped => FlipEndianOfOffsets(reader),
            NeedleArchvieDataOffsetMode.SelfRelative => FixSelfRelativeOffsets(reader),
            _ => throw new ArgumentException("Invalid offset mode", nameof(offsetMode)),
        };

        Resource = CreateResourceInstance(filename);
        Resource.ReadResource(reader);

        if(adjustedReader != reader)
        {
            adjustedReader.Dispose();
        }
    }

    protected abstract T CreateResourceInstance(string filename);


    protected override void WriteBlockData(BinaryObjectWriter writer, string filename, NeedleArchvieDataOffsetMode offsetMode)
    {
        if(Resource == null)
        {
            throw new InvalidOperationException("Model block has no model!");
        }

        Resource.WriteResource(writer, filename);
    }


    public override void ResolveDependencies(IResourceResolver dir)
    {
        Resource?.ResolveDependencies(dir);
    }


    public override string ToString()
    {
        return $"{base.ToString()}: \"{Resource}\"";
    }
}
