namespace SharpNeedle.SurfRide.Draw;

public class Scene : IBinarySerializable<ChunkBinaryOptions>
{
    public int Version { get; set; }
    public string Name { get; set; }
    public int ID { get; set; }
    public uint Flags { get; set; }
    public uint Field10 { get; set; }
    public short DefaultCameraIndex { get; set; }
    public uint BackgroundColor { get; set; }
    public Vector2 FrameSize { get; set; }
    public List<Layer> Layers { get; set; } = new();
    public List<Camera> Cameras { get; set; } = new();
    public UserData UserData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            reader.Align(8);
        
        Name = reader.ReadStringOffset();
        ID = reader.Read<int>();
        Flags = reader.Read<uint>();
        if (options.Version >= 4)
            Field10 = reader.Read<uint>();
        
        var layerCount = reader.Read<int>();
        if (options.Version >= 3)
            reader.Align(8);
        
        Layers.AddRange(reader.ReadObjectArrayOffset<Layer, ChunkBinaryOptions>(options, layerCount));
        var camCount = reader.Read<ushort>();
        DefaultCameraIndex = reader.Read<short>();
        if (options.Version >= 3)
            reader.Align(8);
        
        Cameras.AddRange(reader.ReadObjectArrayOffset<Camera, ChunkBinaryOptions>(options, camCount));
        BackgroundColor = reader.Read<uint>();
        FrameSize = reader.Read<Vector2>();
        if (options.Version >= 3)
            reader.Align(8);
        
        long userDataOffset = reader.ReadOffsetValue();
        if (userDataOffset != 0)
            UserData = reader.ReadObjectAtOffset<UserData, ChunkBinaryOptions>(userDataOffset, options);
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.Write(ID);
        writer.Write(Flags);
        if (options.Version >= 4)
            writer.Write(Field10);

        writer.Write(Layers.Count);
        if (Layers.Count != 0)
            writer.WriteObjectCollectionOffset(options, Layers);
        else
            writer.WriteOffsetValue(0);
        
        writer.Write((short)Cameras.Count);
        writer.Write(DefaultCameraIndex);
        if (options.Version >= 3)
            writer.Align(8);
        
        if (Cameras.Count != 0)
        {
            if (options.Version >= 4)
            {
                writer.WriteOffset(() =>
                {
                    foreach (var camera in Cameras)
                        writer.WriteObject(camera, options);
                }, 16);
            }
            else
            {
                writer.WriteObjectCollectionOffset(options, Cameras);
            }
        }
        else
        {
            writer.WriteOffsetValue(0);
        }
        
        writer.Write(BackgroundColor);
        writer.Write(FrameSize);
        if (options.Version >= 3)
            writer.Align(8);
        
        if (UserData != null)
            writer.WriteObjectOffset(UserData, options);
        else
            writer.WriteOffsetValue(0);
    }

    public Layer GetLayer(string name)
    {
        return Layers.Find(item => item.Name == name);
    }

    public Camera GetCamera(string name)
    {
        return Cameras.Find(item => item.Name == name);
    }
}