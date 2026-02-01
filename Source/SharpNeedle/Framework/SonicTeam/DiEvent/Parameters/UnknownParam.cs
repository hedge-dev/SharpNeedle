namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

public class UnknownParam : BaseParam
{
    public byte[] Data { get; set; } = [];
    public int Size { get; set; }
    public int Type { get; set; }

    public UnknownParam() { }

    public UnknownParam(int size, int type)
    {
        Size = size;
        Type = type;
    }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Data = new byte[Size * 4];
        reader.ReadArray<byte>(Size * 4, Data);
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteArrayFixedLength(Data, Size * 4);
    }

    public override int GetTypeID(GameType game) { return Type; }
}