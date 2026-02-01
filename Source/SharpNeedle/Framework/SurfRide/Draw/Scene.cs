namespace SharpNeedle.Framework.SurfRide.Draw;

using SharpNeedle.Structs;

public class Scene : IBinarySerializable<ChunkBinaryOptions>
{
    public int Version { get; set; }
    public string? Name { get; set; }
    public int ID { get; set; }
    public uint Flags { get; set; }
    public uint Field10 { get; set; }
    public short CurrentCameraIndex { get; set; }
    public Color<byte> BackgroundColor { get; set; }
    public Vector2 Resolution { get; set; }
    public List<Layer> Layers { get; set; } = [];
    public List<Camera> Cameras { get; set; } = [];
    public UserData? UserData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Name = reader.ReadStringOffset();
        ID = reader.ReadInt32();
        Flags = reader.ReadUInt32();
        if (options.Version >= 4)
        {
            Field10 = reader.ReadUInt32();
        }

        int layerCount = reader.ReadInt32();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Layers.AddRange(reader.ReadObjectArrayOffset<Layer, ChunkBinaryOptions>(options, layerCount));
        ushort camCount = reader.ReadUInt16();
        CurrentCameraIndex = reader.ReadInt16();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Cameras.AddRange(reader.ReadObjectArrayOffset<Camera, ChunkBinaryOptions>(options, camCount));
        BackgroundColor = reader.ReadObject<Color<byte>>();
        Resolution = reader.ReadVector2();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        long userDataOffset = reader.ReadOffsetValue();
        if (userDataOffset != 0)
        {
            UserData = reader.ReadObjectAtOffset<UserData, ChunkBinaryOptions>(userDataOffset, options);
        }
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
        writer.WriteInt32(ID);
        writer.WriteUInt32(Flags);
        if (options.Version >= 4)
        {
            writer.WriteUInt32(Field10);
        }

        writer.WriteInt32(Layers.Count);
        if (Layers.Count != 0)
        {
            writer.WriteObjectCollectionOffset(options, Layers);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        writer.WriteInt16((short)Cameras.Count);
        writer.WriteInt16(CurrentCameraIndex);
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        if (Cameras.Count != 0)
        {
            if (options.Version >= 4)
            {
                writer.WriteOffset(() =>
                {
                    foreach (Camera camera in Cameras)
                    {
                        writer.WriteObject(camera, options);
                    }
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

        writer.WriteObject(BackgroundColor);
        writer.WriteVector2(Resolution);
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        if (UserData != null)
        {
            writer.WriteObjectOffset(UserData, options);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }
    }

    public Layer? GetLayer(string name)
    {
        return Layers.Find(item => item.Name == name);
    }

    public Camera? GetCamera(string name)
    {
        return Cameras.Find(item => item.Name == name);
    }
}