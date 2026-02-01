namespace SharpNeedle.Framework.HedgehogEngine.Bullet;

public struct BulletPrimitive : IBinarySerializable
{
    public BulletPrimiteShapeType ShapeType { get; set; }

    public byte SurfaceLayer { get; set; }

    public byte SurfaceType { get; set; }

    public uint SurfaceFlags { get; set; }

    public Vector3 Position { get; set; }

    public Quaternion Rotation { get; set; }

    public Vector3 Dimensions { get; set; }

    public void Read(BinaryObjectReader reader)
    {
        ShapeType = (BulletPrimiteShapeType)reader.ReadByte();
        SurfaceLayer = reader.ReadByte();
        reader.Align(4);

        uint typeFlag = reader.ReadUInt32();
        SurfaceType = (byte)(typeFlag >> 24);
        SurfaceFlags = typeFlag & 0xFFFFFF;

        Position = reader.ReadVector3();
        Rotation = reader.ReadQuaternion();
        Dimensions = reader.ReadVector3();
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.WriteByte((byte)ShapeType);
        writer.WriteByte(SurfaceLayer);
        writer.Align(4);

        uint typeFlag = (SurfaceFlags & 0xFFFFFF) | ((uint)SurfaceType << 24);
        writer.WriteUInt32(typeFlag);

        writer.WriteVector3(Position);
        writer.WriteQuaternion(Rotation);
        writer.WriteVector3(Dimensions);
    }
}
