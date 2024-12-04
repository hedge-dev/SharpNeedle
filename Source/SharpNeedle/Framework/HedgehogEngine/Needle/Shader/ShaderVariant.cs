namespace SharpNeedle.Framework.HedgehogEngine.Needle.Shader;

using System.Linq;

public class ShaderVariant : IBinarySerializable
{
    public byte[] ShaderByteCode { get; set; } = [];

    public byte[] InputSignatureByteCode { get; set; } = [];

    public ShaderGlobalVariables GlobalVariables { get; set; } = new();

    public void Read(BinaryObjectReader reader)
    {
        reader.Skip(4); // shader variant size

        int mainLength = reader.ReadInt32();
        ShaderByteCode = reader.ReadArray<byte>(mainLength);
        reader.Align(4);

        int subLength = reader.ReadInt32();
        InputSignatureByteCode = reader.ReadArray<byte>(subLength);
        reader.Align(4);

        GlobalVariables = reader.ReadObject<ShaderGlobalVariables>();
    }

    public void Write(BinaryObjectWriter writer)
    {
        long startPosition = writer.Position;
        SeekToken sizeToken = writer.At();
        writer.WriteInt32(0);

        writer.WriteInt32(ShaderByteCode.Length);
        writer.WriteArray(ShaderByteCode);
        writer.Align(4);

        writer.WriteInt32(InputSignatureByteCode.Length);
        writer.WriteArray(InputSignatureByteCode);
        writer.Align(4);

        writer.WriteObject(GlobalVariables);

        long endPosition = writer.Position;
        using(SeekToken temp = writer.At())
        {
            sizeToken.Dispose();
            writer.Write((int)(endPosition - startPosition));
        }
    }
}
