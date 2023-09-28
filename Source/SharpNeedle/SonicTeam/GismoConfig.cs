using SharpNeedle.BINA;

namespace SharpNeedle.SonicTeam;

[NeedleResource("st/gismoconfig", @"\.(orc|gism)$")]
public class GismoConfig : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("GISM");
    public uint Version { get; set; } // Guessed
    public List<ObjectData> Objects { get; set; } = new();

    public override void Read(BinaryObjectReader reader)
    {
        if (base.Version.IsV1)
        {
            reader.EnsureSignature(Signature);
        }
        else
        {
            reader.ReadOffset(() =>
                reader.EnsureSignature(Signature));
        }

        Version = reader.Read<uint>();

        int objectCount = reader.Read<int>();
        Objects.AddRange(reader.ReadObjectArrayOffset<ObjectData, bool>(base.Version.IsV1, objectCount));
    }

    public override void Write(BinaryObjectWriter writer)
    {
        if (base.Version.IsV1)
        {
            writer.Write(Signature);
        }
        else
        {
            writer.WriteOffset(() =>
                writer.Write(Signature));
        }

        writer.Write(Version);

        writer.Write(Objects.Count);
        writer.WriteObjectCollectionOffset(base.Version.IsV1, Objects);
    }

    public class ObjectData : IBinarySerializable<bool>
    {
        public string Name { get; set; }
        public string ModelName { get; set; }
        public string SkeletonName { get; set; }

        // Only used in Lost World variant
        public ShapeType ShapeType { get; set; }
        public bool IsMotionOn { get; set; }
        public bool IsProgramMotionOn { get; set; }
        public float CollisionRadius { get; set; }
        public float CollisionHeight { get; set; }
        public Vector3 Size { get; set; } // Only used in Colors
        public MotionData Motion { get; set; } = new();
        public ProgramMotionData ProgramMotion { get; set; } = new();

        // Only used in Colors variant (Maybe related to breaking? Could have overlap with collision fields from Lost World)
        public int Field00 { get; set; }
        public int Field04 { get; set; }
        public short Field08 { get; set; }
        public short Field0A { get; set; }
        public short Field0C { get; set; }
        public byte Field0F { get; set; }
        public int Field10 { get; set; }
        public int Field14 { get; set; }
        public int Field18 { get; set; }
        public int Field1C { get; set; }
        public float Field20 { get; set; }
        public float Field24 { get; set; }
        public float Field28 { get; set; }
        public int Field38 { get; set; }

        public void Read(BinaryObjectReader reader, bool isV1)
        {
            if (isV1)
            {
                Field00 = reader.Read<int>();
                Field04 = reader.Read<int>();
                Field08 = reader.Read<short>();
                Field0A = reader.Read<short>();
                Field0C = reader.Read<short>();

                ShapeType = reader.Read<ShapeType>();

                Field0F = reader.Read<byte>();
                Field10 = reader.Read<int>();
                Field14 = reader.Read<int>();
                Field18 = reader.Read<int>();
                Field1C = reader.Read<int>();
                Field20 = reader.Read<float>();
                Field24 = reader.Read<float>();
                Field28 = reader.Read<float>();

                Size = reader.Read<Vector3>(); // TODO: Unpack reading all 3 float values at once into individual reads based on ShapeType

                Field38 = reader.Read<int>();
            }

            Name = reader.ReadStringOffset();
            ModelName = reader.ReadStringOffset();
            SkeletonName = reader.ReadStringOffset();

            if (!isV1)
            {
                ShapeType = reader.Read<ShapeType>();
                CollisionRadius = reader.Read<float>();
                CollisionHeight = reader.Read<float>();

                IsMotionOn = Convert.ToBoolean(reader.Read<int>());
                Motion = reader.ReadObjectOffset<MotionData>();

                IsProgramMotionOn = Convert.ToBoolean(reader.Read<int>());
                ProgramMotion = reader.ReadObjectOffset<ProgramMotionData>();
            }
        }

        public void Write(BinaryObjectWriter writer, bool isV1)
        {

            if (isV1)
            {
                writer.Write(Field00);
                writer.Write(Field04);
                writer.Write(Field08);
                writer.Write(Field0A);
                writer.Write(Field0C);

                writer.Write(ShapeType);

                writer.Write(Field0F);
                writer.Write(Field10);
                writer.Write(Field14);
                writer.Write(Field18);
                writer.Write(Field1C);
                writer.Write(Field20);
                writer.Write(Field24);
                writer.Write(Field28);

                writer.Write(Size); // TODO: Unpack writing all 3 float values at once into individual reads based on ShapeType

                writer.Write(Field38);
            }

            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ModelName);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, SkeletonName);

            if (!isV1)
            {
                writer.Write(ShapeType);
                writer.Write(CollisionRadius);
                writer.Write(CollisionHeight);

                writer.Write(Convert.ToInt32(IsMotionOn));
                writer.WriteObjectOffset(Motion);

                writer.Write(Convert.ToInt32(IsProgramMotionOn));
                writer.WriteObjectOffset(ProgramMotion);
            }
        }
    }

    public class MotionData : IBinarySerializable
    {
        public string AnimationName { get; set; }
        public PlayPolicy PlayPolicy { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            PlayPolicy = reader.Read<PlayPolicy>();
            AnimationName = reader.ReadStringOffset();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(PlayPolicy);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, AnimationName);
        }
    }

    public class ProgramMotionData : IBinarySerializable
    {
        public MotionType MotionType { get; set; }
        public int Field00 { get; set; }
        public float Power { get; set; }
        public float SpeedScale { get; set; }
        public float Time { get; set; }
        public Vector3 Axis { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            Field00 = reader.Read<int>();
            
            MotionType = reader.Read<MotionType>();

            Axis = reader.Read<Vector3>();

            Power = reader.Read<float>();
            SpeedScale = reader.Read<float>();
            Time = reader.Read<float>();
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Field00);

            writer.Write(MotionType);

            writer.Write(Axis);

            writer.Write(Power);
            writer.Write(SpeedScale);
            writer.Write(Time);
        }
    }
}

public enum ShapeType : int
{
    Sphere = 1,
    Capsule,
    Box = 4 // Unused in Lost World
}

public enum PlayPolicy : int
{

}

public enum MotionType : int
{
    Swing,
    Rotate
}