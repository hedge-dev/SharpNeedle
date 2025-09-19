namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public class Animation : IBinarySerializable
{
    public string TypeName;
    public Track ColorA;
    public Track ColorB;
    public Track ColorG;
    public Track ColorR;
    
    public Track TransX;
    public Track TransY;
    public Track TransZ;
    
    public Track ScaleX;
    public Track ScaleY;
    public Track ScaleZ;
    
   
    public void Read(BinaryObjectReader reader)
    {        
        TypeName = reader.ReadStringPaddedByte();

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