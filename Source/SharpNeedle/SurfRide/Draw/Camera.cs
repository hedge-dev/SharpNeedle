namespace SharpNeedle.SurfRide.Draw;

public class Camera : IBinarySerializable<ChunkBinaryOptions>
{
    public string Name { get; set; }
    public int ID { get; set; }
    public uint Flags { get; set; }
    public float Near { get; set; }
    public float Far { get; set; }
    public long Field48 { get; set; }
    public long Field50 { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 LookAt { get; set; }
    public int Field38 { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        if (options.Version >= 1)
        {
            if (options.Version >= 3)
                reader.Align(8);
            
            Name = reader.ReadStringOffset();
            ID = reader.Read<int>();
        }
        
        if (options.Version >= 4)
        {
            reader.Align(16);
            Position = reader.Read<Vector3>();
            reader.Align(16);
            LookAt = reader.Read<Vector3>();
            reader.Align(16);
            Field38 = reader.Read<int>();
        }
        else
        {
            Position = reader.Read<Vector3>();
            LookAt = reader.Read<Vector3>();
        }
        
        Flags = reader.Read<uint>();
        Near = reader.Read<float>();
        Far = reader.Read<float>();
        if (options.Version >= 1)
        {
            Field48 = reader.ReadOffsetValue();
            Field50 = reader.ReadOffsetValue();
        }
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (options.Version >= 1)
        {
            if (options.Version >= 3)
                writer.Align(8);
            
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.Write(ID);
        }
        
        if (options.Version >= 4)
        {
            writer.Align(16);
            writer.Write(Position);
            writer.Align(16);
            writer.Write(LookAt);
            writer.Align(16);
            writer.Write(Field38);
        }
        else
        {
            writer.Write(Position);
            writer.Write(LookAt);
        }
        
        writer.Write(Flags);
        writer.Write(Near);
        writer.Write(Far);
        if (options.Version >= 1)
        {
            writer.WriteOffsetValue(Field48);
            writer.WriteOffsetValue(Field50);
        }
    }
}
