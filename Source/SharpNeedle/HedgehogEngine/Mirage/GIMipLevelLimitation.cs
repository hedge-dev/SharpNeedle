namespace SharpNeedle.HedgehogEngine.Mirage;

// ReSharper disable once InconsistentNaming
[BinaryResource("hh/gil", @"\.gil$")]
public class GIMipLevelLimitation : SampleChunkResource
{
    public bool Level0 { get; set; }
    public bool Level1 { get; set; }
    public bool Level2 { get; set; }

    public override void Read(BinaryObjectReader reader)
    {
        Level0 = reader.Read<bool>();
        Level1 = reader.Read<bool>();
        Level2 = reader.Read<bool>();
        reader.Align(4);
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Level0);
        writer.Write(Level1);
        writer.Write(Level2);
        writer.Align(4);
    }

    public bool this[int index]
    {
        get
        {
            switch (index)
            {
                case 0: return Level0;
                case 1: return Level1;
                case 2: return Level2;
                default:
                    throw new IndexOutOfRangeException($"{index} >= 3");
            }
        }

        set
        {
            switch (index)
            {
                case 0: Level0 = value; break;
                case 1: Level1 = value; break;
                case 2: Level2 = value; break;
                default:
                    throw new IndexOutOfRangeException($"{index} >= 3");
            }
        }
    }

    public override string ToString()
    {
        return $"Level0: {Level0}, Level1: {Level1}, Level2: {Level2}";
    }
}