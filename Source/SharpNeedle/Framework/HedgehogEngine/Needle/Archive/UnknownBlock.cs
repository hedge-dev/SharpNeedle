namespace SharpNeedle.Framework.HedgehogEngine.Needle.Archive;

using Amicitia.IO.Binary;

public class UnknownBlock : NeedleArchiveBlock
{
    public override string Signature => BlockSignature;

    public string BlockSignature { get; set; } = "NEDUNK"; // fallback

    public byte[] Data { get; set; } = [];

    public UnknownBlock(string signature) : base("unknown")
    {
        BlockSignature = signature;
    }


    protected override bool IsVersionValid(int version)
    {
        return true;
    }

    protected override void ReadBlockData(BinaryObjectReader reader, string filename, NeedleArchvieDataOffsetMode offsetMode)
    {
        Data = reader.ReadArray<byte>((int)reader.Length);
    }

    protected override void WriteBlockData(BinaryObjectWriter writer, string filename, NeedleArchvieDataOffsetMode offsetMode)
    {
        writer.WriteArray(Data);
    }

}
