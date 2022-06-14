namespace SharpNeedle.SurfRide.Draw.Animation;

public class KeyHermite : IKeyFrame
{
    public int Frame { get; set; }
    public Union Value { get; set; }
    public float TangentIn { get; set; }
    public float TangentOut { get; set; }

    public void Read(BinaryObjectReader reader, uint flags)
    {
        Frame = reader.Read<int>();
        switch (flags & 0xF0)
        {
            case 0x10:
                Value = reader.Read<float>();
                break;
            case 0x20:
                Value = reader.Read<int>();
                break;
            case 0x40:
                Value = reader.Read<int>();
                break;
            case 0x60:
                Value = reader.Read<uint>();
                break;
            case 0x70:
                Value = reader.Read<double>();
                break;
            default:
                throw new NotImplementedException();
        }
        TangentIn = reader.Read<float>();
        TangentOut = reader.Read<float>();
    }

    public void Write(BinaryObjectWriter writer, uint flags)
    {
        writer.Write(Frame);
        switch (flags & 0xF0)
        {
            case 0x10:
                writer.Write(Value.Float);
                break;
            case 0x20:
                writer.Write(Value.Integer);
                break;
            case 0x40:
                writer.Write(Value.Integer);
                break;
            case 0x60:
                writer.Write(Value.UnsignedInteger);
                break;
            case 0x70:
                writer.Write(Value.Double);
                break;
            default:
                throw new NotImplementedException();
        }
        writer.Write(TangentIn);
        writer.Write(TangentOut);
    }
}
