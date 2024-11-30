namespace SharpNeedle.LostWorld.Animation;
using System.Globalization;

public struct TriggerInfo : IBinarySerializable
{
    public uint ID;
    public float Frame;
    public CallbackParam Param;

    public void Read(BinaryObjectReader reader)
    {
        reader.Read(out Frame);
        reader.Read(out ID);
        reader.ReadObject(ref Param);
    }

    public void Write(BinaryObjectWriter writer)
    {
        writer.Write(ref Frame);
        writer.Write(ref ID);
        Param.Write(writer);
    }

    public struct CallbackParam : IBinarySerializable
    {
        // Combined type and param to one struct for convenience
        public TriggerValueType Type;
        public int Integer;
        public float Float;
        public string String;

        public CallbackParam(int value)
        {
            Type = TriggerValueType.Int;
            Float = default;
            String = default;
            Integer = value;
        }

        public CallbackParam(float value)
        {
            Type = TriggerValueType.Float;
            String = default;
            Integer = default;
            Float = value;
        }

        public CallbackParam(string value)
        {
            Type = TriggerValueType.String;
            Integer = default;
            Float = default;
            String = value;
        }

        public void Read(BinaryObjectReader reader)
        {
            reader.Read(out Type);
            switch(Type)
            {
                case TriggerValueType.Enum:
                case TriggerValueType.Int:
                    reader.Read(out Integer);
                    break;

                case TriggerValueType.Float:
                    reader.Read(out Float);
                    break;

                case TriggerValueType.String:
                    String = reader.ReadStringOffset();
                    break;
            }
        }

        public void Write(BinaryObjectWriter writer)
        {
            writer.Write(Type);
            switch(Type)
            {
                case TriggerValueType.Enum:
                case TriggerValueType.Int:
                    writer.Write(Integer);
                    break;

                case TriggerValueType.Float:
                    writer.Write(ref Float);
                    break;

                case TriggerValueType.String:
                    writer.WriteStringOffset(StringBinaryFormat.NullTerminated, String);
                    break;
            }
        }

        public override readonly string ToString()
        {
            switch(Type)
            {
                case TriggerValueType.Int:
                case TriggerValueType.Enum:
                    return Integer.ToString();

                case TriggerValueType.Float:
                    return Float.ToString(CultureInfo.InvariantCulture);

                case TriggerValueType.String:
                    return String;
            }

            return base.ToString();
        }

        public static implicit operator CallbackParam(int value)
        {
            return new(value);
        }

        public static implicit operator CallbackParam(float value)
        {
            return new(value);
        }

        public static implicit operator CallbackParam(string value)
        {
            return new(value);
        }
    }
}

public enum TriggerValueType
{
    Enum,
    Int,
    Float,
    String
}