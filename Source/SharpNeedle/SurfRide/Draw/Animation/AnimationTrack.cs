namespace SharpNeedle.SurfRide.Draw.Animation;

public class AnimationTrack : List<IKeyFrame>, IBinarySerializable<ChunkBinaryOptions>
{
    public ushort Field00 { get; set; }
    public uint Flags { get; set; }
    public uint StartFrame { get; set; }
    public uint EndFrame { get; set; }

    public void Read(BinaryObjectReader reader, ChunkBinaryOptions options)
    {
        Clear();
        Field00 = reader.Read<ushort>();
        Capacity = reader.Read<ushort>();
        Flags = reader.Read<uint>();
        StartFrame = reader.Read<uint>();
        EndFrame = reader.Read<uint>();
        if (options.Version >= 3)
            reader.Align(8);
        
        switch (Flags & 3)
        {
            case 0:
                AddRange(reader.ReadObjectArrayOffset<KeyConstant, uint>(Flags, Capacity));
                break;
            case 1:
                AddRange(reader.ReadObjectArrayOffset<KeyLinear, uint>(Flags, Capacity));
                break;
            case 2:
                AddRange(reader.ReadObjectArrayOffset<KeyHermite, uint>(Flags, Capacity));
                break;
            case 3:
                AddRange(reader.ReadObjectArrayOffset<KeyIndividual, uint>(Flags, Capacity));
                break;
            default:
                throw new NotImplementedException();
        }
    }

    public void Write(BinaryObjectWriter writer, ChunkBinaryOptions options)
    {
        writer.Write(Field00);
        writer.Write((ushort)Count);
        writer.Write(Flags);
        writer.Write(StartFrame);
        writer.Write(EndFrame);
        if (options.Version >= 3)
            writer.Align(8);
        
        writer.WriteObjectCollectionOffset(Flags, this);
    }
}
