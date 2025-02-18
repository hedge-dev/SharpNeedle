namespace SharpNeedle.Framework.HedgehogEngine.Needle.Archive;

using SharpNeedle.Framework.HedgehogEngine.Mirage;
using SharpNeedle.IO;
using System;
using System.IO;
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

        for (int i = 0; i < rawOffsets.Length; i++)
        {
            result[i] = rawOffsets[i] + startPosition + 0x10;
        }

        return result;
    }

    private static void FlipEndianOfOffsets(long[] offsets, byte[] resourceData)
    {
        foreach (long offset in offsets)
        {
            (resourceData[offset], resourceData[offset + 3]) = (resourceData[offset + 3], resourceData[offset]);
            (resourceData[offset + 1], resourceData[offset + 2]) = (resourceData[offset + 2], resourceData[offset + 1]);
        }
    }

    private static void SelfRelativeToAbsoluteOffsets(long[] offsets, byte[] resourceData)
    {
        using (MemoryStream stream = new(resourceData))
        {
            using BinaryObjectReader reader = new(stream, StreamOwnership.Retain, Endianness.Little);
            using BinaryObjectWriter writer = new(stream, StreamOwnership.Retain, Endianness.Big);

            foreach (uint offset in offsets)
            {
                reader.Seek(offset, SeekOrigin.Begin);
                int relativeOffset = reader.ReadInt32();
                uint realOffset = (uint)(relativeOffset + offset - 0x10);

                writer.Seek(offset, SeekOrigin.Begin);
                writer.WriteUInt32(realOffset);
            }

            writer.Flush();
        }
    }

    private static void AbsoluteToSelfRelativeOffsets(long[] offsets, byte[] resourceData)
    {
        using (MemoryStream stream = new(resourceData))
        {
            using BinaryObjectReader reader = new(stream, StreamOwnership.Retain, Endianness.Big);
            using BinaryObjectWriter writer = new(stream, StreamOwnership.Retain, Endianness.Little);

            foreach (uint offset in offsets)
            {
                reader.Seek(offset, SeekOrigin.Begin);
                uint realOffset = reader.ReadUInt32();
                int relativeOffset = (int)(realOffset - offset + 0x10);

                writer.Seek(offset, SeekOrigin.Begin);
                writer.WriteInt32(relativeOffset);
            }

            writer.Flush();
        }
    }

    protected override void ReadBlockData(BinaryObjectReader reader, string filename, NeedleArchiveDataOffsetMode offsetMode)
    {
        BinaryObjectReader adjustedReader = reader;

        if (offsetMode != NeedleArchiveDataOffsetMode.Default)
        {
            using (SeekToken startToken = reader.At())
            {
                long[] offsets = GetOffsetTable(reader);
                byte[] adjustedData = reader.GetBaseStream().ReadAllBytes();

                switch (offsetMode)
                {
                    case NeedleArchiveDataOffsetMode.Flipped:
                        FlipEndianOfOffsets(offsets, adjustedData);
                        break;
                    case NeedleArchiveDataOffsetMode.SelfRelative:
                        SelfRelativeToAbsoluteOffsets(offsets, adjustedData);
                        break;
                }

                MemoryStream stream = new(adjustedData);
                adjustedReader = new(stream, StreamOwnership.Transfer, reader.Endianness);
            }
        }

        Resource = CreateResourceInstance(filename);
        Resource.ReadResource(adjustedReader);

        if (adjustedReader != reader)
        {
            adjustedReader.Dispose();
        }
    }

    protected abstract T CreateResourceInstance(string filename);



    protected override void WriteBlockData(BinaryObjectWriter writer, string filename, NeedleArchiveDataOffsetMode offsetMode)
    {
        if (Resource == null)
        {
            throw new InvalidOperationException("Model block has no model!");
        }

        byte[] resourceData;

        using MemoryStream resourceStream = new();
        {
            using BinaryObjectWriter resourceWriter = new(resourceStream, StreamOwnership.Retain, Endianness.Big);

            Resource.WriteResource(resourceWriter, filename);
            resourceWriter.Flush();
            resourceData = resourceStream.ToArray();
        }

        if (offsetMode != NeedleArchiveDataOffsetMode.Default)
        {
            long[] offsets;
            using (MemoryStream offsetsStream = new(resourceData))
            {
                using BinaryObjectReader offsetsReader = new(offsetsStream, StreamOwnership.Retain, Endianness.Big);
                offsets = GetOffsetTable(offsetsReader);
            }

            switch (offsetMode)
            {
                case NeedleArchiveDataOffsetMode.Flipped:
                    FlipEndianOfOffsets(offsets, resourceData);
                    break;
                case NeedleArchiveDataOffsetMode.SelfRelative:
                    AbsoluteToSelfRelativeOffsets(offsets, resourceData);
                    break;
            }
        }

        writer.WriteBytes(resourceData);
    }


    public override void ResolveDependencies(IResourceResolver dir)
    {
        Resource?.ResolveDependencies(dir);
    }


    public override void WriteDependencies(IDirectory dir)
    {
        Resource?.WriteDependencies(dir);
    }


    public override string ToString()
    {
        return $"{base.ToString()}: \"{Resource}\"";
    }
}
