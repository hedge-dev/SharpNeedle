namespace SharpNeedle.Framework.SurfRide.Draw;

public class Layer : IBinarySerializable<ChunkBinaryOptions>
{
    public string? Name { get; set; }
    public int ID { get; set; }
    public uint Flags { get; set; }
    public uint CurrentAnimationIndex { get; set; }
    public List<CastNode> Nodes { get; set; } = [];
    public List<ICell> Cells { get; set; } = [];
    public List<Animation> Animations { get; set; } = [];
    public UserData? UserData { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Name = reader.ReadStringOffset();
        ID = reader.Read<int>();
        Flags = reader.Read<uint>();
        int castCount = reader.Read<int>();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Nodes.AddRange(reader.ReadObjectArrayOffset<CastNode, ChunkBinaryOptions>(options, castCount));
        switch (Flags & 0xF)
        {
            case 0:
                Cells.AddRange(reader.ReadObjectArrayOffset<Cell2D, ChunkBinaryOptions>(options, castCount));
                break;

            case 1:
                Cells.AddRange(reader.ReadObjectArrayOffset<Cell3D, ChunkBinaryOptions>(options, castCount));
                break;

            default:
                throw new NotImplementedException();
        }

        int animCount = reader.Read<int>();
        if (options.Version >= 3)
        {
            reader.Align(8);
        }

        Animations.AddRange(reader.ReadObjectArrayOffset<Animation, ChunkBinaryOptions>(options, animCount));
        CurrentAnimationIndex = reader.Read<uint>();
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
        writer.Write(ID);
        writer.Write(Flags);
        writer.Write(Nodes.Count);
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        if (Nodes.Count != 0)
        {
            writer.WriteObjectCollectionOffset(options, Nodes);
            if (options.Version >= 4)
            {
                writer.WriteOffset(() =>
                {
                    foreach (ICell cell in Cells)
                    {
                        writer.WriteObject(cell, options);
                    }
                }, 16);
            }
            else
            {
                writer.WriteObjectCollectionOffset(options, Cells);
            }
        }
        else
        {
            writer.WriteOffsetValue(0);
            writer.WriteOffsetValue(0);
        }

        writer.Write(Animations.Count);
        if (options.Version >= 3)
        {
            writer.Align(8);
        }

        if (Animations.Count != 0)
        {
            writer.WriteObjectCollectionOffset(options, Animations);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }

        writer.Write(CurrentAnimationIndex);
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

    public CastNode? GetCastNode(string? name)
    {
        return Nodes.Find(item => item.Name == name);
    }

    public ICell? GetCastCell(string? name)
    {
        int index = Nodes.FindIndex(item => item.Name == name);
        if (index == -1)
        {
            return null;
        }

        return Cells[index];
    }

    public Animation? GetAnimation(string? name)
    {
        return Animations.Find(item => item.Name == name);
    }
}
