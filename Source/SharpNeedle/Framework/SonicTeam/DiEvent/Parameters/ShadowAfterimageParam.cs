namespace SharpNeedle.Framework.SonicTeam.DiEvent.Parameters;

using SharpNeedle.Structs;

public class ShadowAfterimageParam : BaseParam
{
    public Color<int> Color { get; set; }
    public int Field10 { get; set; } // Possibly afterimage count?
    public int Field14 { get; set; }
    public int Field18 { get; set; }
    public int Field1C { get; set; }
    public int Field20 { get; set; }
    public float Field24 { get; set; }
    public float Field28 { get; set; }
    public int Field2C { get; set; }

    public ShadowAfterimageParam() { }

    public override void Read(BinaryObjectReader reader, GameType game)
    {
        Color = reader.ReadObject<Color<int>>();
        Field10 = reader.ReadInt32();
        Field14 = reader.ReadInt32();
        Field18 = reader.ReadInt32();
        Field1C = reader.ReadInt32();
        Field20 = reader.ReadInt32();
        Field24 = reader.ReadSingle();
        Field28 = reader.ReadSingle();
        Field2C = reader.ReadInt32();
    }

    public override void Write(BinaryObjectWriter writer, GameType game)
    {
        writer.WriteObject(Color);
        writer.WriteInt32(Field10);
        writer.WriteInt32(Field14);
        writer.WriteInt32(Field18);
        writer.WriteInt32(Field1C);
        writer.WriteInt32(Field20);
        writer.WriteSingle(Field24);
        writer.WriteSingle(Field28);
        writer.WriteInt32(Field2C);
    }

    public override int GetTypeID(GameType game)
    {
        switch (game)
        {
            case GameType.ShadowGenerations:
                return (int)ShadowGensParams.ShadowAfterimage;

            default:
                return 0;
        }
    }
}