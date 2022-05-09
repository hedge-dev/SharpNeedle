namespace SharpNeedle.Ninja.Csd;
using System.Buffers.Binary;
using System.IO;

public class CsdPackage : ResourceBase, IBinarySerializable
{
    public static readonly uint Signature = BinaryHelper.MakeSignature<uint>("FAPC");
    public List<byte[]> Files { get; set; } = new();
    public Endianness Endianness { get; set; } = BinaryHelper.PlatformEndianness;

    public Stream GetStream(int idx, bool writable = false, bool expandable = false)
    {
        if (!writable)
            return new MemoryStream(Files[idx]);

        if (!expandable)
            return new MemoryStream(Files[idx], true);

        // Make a new buffer because we want to expand it
        var stream = new WrappedStream<MemoryStream>(new MemoryStream(Files[idx].Length));
        stream.Write(Files[idx]);
        stream.OnClosing += (x) =>
        {
            Files[idx] = x.ToArray();
        };

        return stream;
    }

    public int Add()
    {
        Add(Array.Empty<byte>());
        return Files.Count - 1;
    }

    public void Add(byte[] data)
    {
        Files.Add(data);
    }

    public override void Read(IFile file)
    {
        Name = file.Name;
        BaseFile = file;
        using var reader = new BinaryObjectReader(file.Open(), StreamOwnership.Transfer, Endianness.Little);
        Read(reader);
    }

    public override void Write(IFile file)
    {
        BaseFile = file;
        using var writer = new BinaryObjectWriter(file.Open(), StreamOwnership.Retain, Endianness.Little);
        Write(writer);
    }

    public void Read(BinaryObjectReader reader)
    {
        var sig = reader.ReadNative<uint>();
        if (sig == BinaryPrimitives.ReverseEndianness(Signature))
            Endianness = Endianness.Big;

        reader.Endianness = Endianness;
        while (reader.Position < reader.Length)
        {
            Files.Add(reader.ReadArray<byte>(reader.Read<int>()));
        }
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Endianness = Endianness;
        writer.Write(Signature);
        foreach (var file in Files)
        {
            writer.Write(file.Length);
            writer.WriteArray(file);
        }
    }
}