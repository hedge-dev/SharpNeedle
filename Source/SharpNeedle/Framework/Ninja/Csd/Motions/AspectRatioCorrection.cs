namespace SharpNeedle.Framework.Ninja.Csd.Motions;

[StructLayout(LayoutKind.Sequential)]
public struct AspectRatioCorrection : IBinarySerializable
{
    public Vector2 Center;
    public float Offset;

    public void Read(BinaryObjectReader reader)
    {
        Center = reader.ReadVector2();
        Offset = reader.ReadSingle();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteVector2(Center);
        writer.WriteSingle(Offset);
    }
}