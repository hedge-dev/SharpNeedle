using System.IO;

namespace SharpNeedle.BINA;

public abstract class BinaryResource : ResourceBase, IBinarySerializable
{
    public static readonly uint Signature = BinaryHelper.MakeSignature<uint>("BINA");

    public Version Version { get; set; }
    public uint Size { get; set; }
    public List<IChunk> Chunks { get; set; } = new();

    public override void Read(IFile file)
    {
        Name = Path.GetFileNameWithoutExtension(file.Name);
        using var reader = new BinaryObjectReader(file.Open(), StreamOwnership.Transfer, Endianness.Little);
        
        reader.EnsureSignatureNative(Signature);
        Version = reader.ReadObject<Version>();
        reader.Endianness = Version.Endianness;

        Size = reader.Read<uint>();
        var chunkCount = reader.Read<ushort>();
        reader.Skip(2);

        for (int i = 0; i < chunkCount; i++)
        {
            var header = reader.ReadObject<ChunkHeader>();

            if (header.Signature == DataChunk.BinSignature || header.Signature == DataChunk.AltBinSignature)
            {
                var chunk = new DataChunk<IBinarySerializable>(this);
                chunk.Read(reader, header);
                Chunks.Add(chunk);
                continue;
            }

            Chunks.Add(reader.ReadObject<RawChunk, ChunkHeader?>(header));
        }
    }

    public override void Write(IFile file)
    {
        using var writer = new BinaryObjectWriter(file.Open(FileAccess.Write), StreamOwnership.Transfer, Version.Endianness);
        var begin = writer.Position;
        writer.Write(Signature);
        writer.WriteObject(Version);

        var sizeToken = writer.At();
        writer.Write(0);
        writer.Write((short)Chunks.Count);
        writer.Align(4);

        foreach (var chunk in Chunks)
            writer.WriteObject(chunk);

        var end = writer.At();
        sizeToken.Dispose();

        writer.Write((int)((long)end - begin));
    }

    public abstract void Read(BinaryObjectReader reader);

    public abstract void Write(BinaryObjectWriter writer);
}