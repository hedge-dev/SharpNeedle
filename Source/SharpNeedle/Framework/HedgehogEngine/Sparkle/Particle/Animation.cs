namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public class Animation : IBinarySerializable
{
    public Track ColorA { get; set; }
    public Track ColorB { get; set; }
    public Track ColorG { get; set; }
    public Track ColorR { get; set; }

    public Track TransX { get; set; }
    public Track TransY { get; set; }
    public Track TransZ { get; set; }

    public Track ScaleX { get; set; }
    public Track ScaleY { get; set; }
    public Track ScaleZ { get; set; }


    public void Read(BinaryObjectReader reader)
    {
        //AnimationChunk
        reader.ReadStringPaddedByte();

        ColorA = reader.ReadObject<Track>();
        ColorB = reader.ReadObject<Track>();
        ColorG = reader.ReadObject<Track>();
        ColorR = reader.ReadObject<Track>();

        TransX = reader.ReadObject<Track>();
        TransY = reader.ReadObject<Track>();
        TransZ = reader.ReadObject<Track>();

        ScaleX = reader.ReadObject<Track>();
        ScaleY = reader.ReadObject<Track>();
        ScaleZ = reader.ReadObject<Track>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteStringPaddedByte("AnimationChunk");

        writer.WriteObject(ColorA);
        writer.WriteObject(ColorB);
        writer.WriteObject(ColorG);
        writer.WriteObject(ColorR);

        writer.WriteObject(TransX);
        writer.WriteObject(TransY);
        writer.WriteObject(TransZ);

        writer.WriteObject(ScaleX);
        writer.WriteObject(ScaleY);
        writer.WriteObject(ScaleZ);
    }
}