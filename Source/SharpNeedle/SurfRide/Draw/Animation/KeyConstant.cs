namespace SharpNeedle.SurfRide.Draw.Animation;

public class KeyConstant : IKeyFrame
{
    public int Frame { get; set; }
    public Union Value { get; set; }

    public void Read(BinaryObjectReader reader, uint flags)
    {
        Frame = reader.Read<int>();
        switch (flags & 0xF0)
        {
            case 0x10:
                Value = reader.Read<float>();
                break;
            case 0x20:
            case 0x40:
                Value = reader.Read<int>();
                break;
            case 0x30:
                Value = reader.Read<bool>();
                break;
            case 0x50:
                Value = reader.Read<Color<byte>>();
                break;
            case 0x60:
                Value = reader.Read<uint>();
                break;
            case 0x70:
                Value = reader.Read<double>();
                break;
            case 0x80:
                Value = reader.Read<char>();
                break;
            default:
                throw new NotImplementedException();
        }
        reader.Align(4);
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
            case 0x40:
                writer.Write(Value.Integer);
                break;
            case 0x30:
                writer.Write(Value.Boolean);
                break;
            case 0x50:
                writer.Write(Value.Color);
                break;
            case 0x60:
                writer.Write(Value.UnsignedInteger);
                break;
            case 0x70:
                writer.Write(Value.Double);
                break;
            case 0x80:
                writer.Write(Value.Character);
                break;
            default:
                throw new NotImplementedException();
        }
        writer.Align(4);
    }
}
