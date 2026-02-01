namespace SharpNeedle.Framework.SurfRide.Draw;

public class Camera : IBinarySerializable<ChunkBinaryOptions>
{
    public string? Name { get; set; }
    public int ID { get; set; }
    public uint FieldOfView { get; set; }
    public float NearPlane { get; set; }
    public float FarPlane { get; set; }
    public int Field30 { get; set; }
    public long Field48 { get; set; }
    public long Field50 { get; set; }
    public Vector3 Position { get; set; }
    public Vector3 Target { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        if (options.Version >= 1)
        {
            if (options.Version >= 3)
            {
                reader.Align(8);
            }

            Name = reader.ReadStringOffset();
            ID = reader.ReadInt32();
        }

        if (options.Version >= 4)
        {
            reader.Align(16);
            Position = reader.ReadVector3();
            reader.Align(16);
            Target = reader.ReadVector3();
            reader.Align(16);
            Field30 = reader.ReadInt32();
        }
        else
        {
            Position = reader.ReadVector3();
            Target = reader.ReadVector3();
        }

        FieldOfView = reader.ReadUInt32();
        NearPlane = reader.ReadSingle();
        FarPlane = reader.ReadSingle();
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
            {
                writer.Align(8);
            }

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.WriteInt32(ID);
        }

        if (options.Version >= 4)
        {
            writer.Align(16);
            writer.WriteVector3(Position);
            writer.Align(16);
            writer.WriteVector3(Target);
            writer.Align(16);
            writer.WriteInt32(Field30);
        }
        else
        {
            writer.WriteVector3(Position);
            writer.WriteVector3(Target);
        }

        writer.WriteUInt32(FieldOfView);
        writer.WriteSingle(NearPlane);
        writer.WriteSingle(FarPlane);
        if (options.Version >= 1)
        {
            writer.WriteOffsetValue(Field48);
            writer.WriteOffsetValue(Field50);
        }
    }
}
