namespace SharpNeedle.SurfRide.Draw;

public class Camera : IBinarySerializable<ChunkBinaryOptions>
{
    public string Name { get; set; }
    public int ID { get; set; }
    public uint Flags { get; set; }
    public float NearPlane { get; set; }
    public float FarPlane { get; set; }
    public long Field30 { get; set; }
    public long Field48 { get; set; }
    public long Field50 { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Target { get; set; }

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
            Target = reader.Read<Vector3>();
            reader.Align(16);
            Field30 = reader.Read<int>();
        }
        else
        {
            Position = reader.Read<Vector3>();
            Target = reader.Read<Vector3>();
        }
        
        Flags = reader.Read<uint>();
        NearPlane = reader.Read<float>();
        FarPlane = reader.Read<float>();
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
            writer.Write(Target);
            writer.Align(16);
            writer.Write(Field30);
        }
        else
        {
            writer.Write(Position);
            writer.Write(Target);
        }
        
        writer.Write(Flags);
        writer.Write(NearPlane);
        writer.Write(FarPlane);
        if (options.Version >= 1)
        {
            writer.WriteOffsetValue(Field48);
            writer.WriteOffsetValue(Field50);
        }
    }
}
