namespace SharpNeedle.Framework.HedgehogEngine.Needle.Shader;

using Amicitia.IO.Binary;
using SharpNeedle.Framework.BINA;
using SharpNeedle.Framework.HedgehogEngine.Mirage;
using System.Drawing;

[NeedleResource("hhn/shader-list", @"\.shader-list$")]
public class ShaderList : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("NDSL");

    public struct Shader : IBinarySerializable
    {
        public string? TypeName { get; set; }
        public string? ShaderName { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            TypeName = reader.ReadStringOffset();
            ShaderName = reader.ReadStringOffset();
        }

        public readonly void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, TypeName);
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, ShaderName);
        }

        public override string ToString()
        {
            return $"{TypeName}: {ShaderName}";
        }
    }

    public struct Input : IBinarySerializable
    {
        public string? Name { get; set; }
        public int ID { get; set; }

        public void Read(BinaryObjectReader reader)
        {
            Name = reader.ReadStringOffset();
            ID = reader.Read<int>();
            reader.Align(8);
        }

        public readonly void Write(BinaryObjectWriter writer)
        {
            writer.WriteStringOffset(StringBinaryFormat.NullTerminated, Name);
            writer.Write(ID);
            writer.Align(8);
        }

        public override string ToString()
        {
            return $"{Name}: {ID}";
        }
    }


    public int ShaderListVersion { get; set; } = 2;

    public List<Shader> Shaders { get; set; } = [];

    public List<Input> Inputs { get; set; } = [];


    public override void Read(BinaryObjectReader reader)
    {
        BinaryHelper.EnsureSignature(reader.ReadNative<uint>(), true, Signature);
        ShaderListVersion = reader.ReadInt32();

        long inputsOffset = reader.ReadOffsetValue();
        int inputsSize = reader.ReadInt32();
        reader.Skip(4);

        long shadersOffset = reader.ReadOffsetValue();
        int shadersSize = reader.ReadInt32();

        Inputs = reader.ReadObjectArrayAtOffset<Input>(inputsOffset, inputsSize).ToList();
        Shaders = reader.ReadObjectArrayAtOffset<Shader>(shadersOffset, shadersSize).ToList();
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.WriteUInt32(Signature);
        writer.WriteInt32(ShaderListVersion);

        writer.WriteOffset(() =>
        {
            foreach(Input input in Inputs)
            {
                writer.WriteObject(input);
            }
        });
        writer.WriteInt32(Inputs.Count);
        writer.Align(8);

        writer.WriteOffset(() =>
        {
            foreach(Shader input in Shaders)
            {
                writer.WriteObject(input);
            }
        });
        writer.WriteInt32(Shaders.Count);

        // the weirdest aligment padding in existence.
        // Only done when any of the arrays have values,
        // and it affects the pointers too.
        // Way to save 4 bytes i guess.
        if(Shaders.Count > 0 || Inputs.Count > 0)
        {
            writer.Align(8);
        }
    }
}
