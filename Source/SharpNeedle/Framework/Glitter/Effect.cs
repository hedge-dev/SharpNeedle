namespace SharpNeedle.Framework.Glitter;

using SharpNeedle.Framework.BINA;

[NeedleResource("glitter/effect", @"\.(gle|effect)$")]
public class Effect : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("FIRG");
    public new uint Version { get; set; } = 0x01060000;
    public LinkedList<EffectParameter> Parameters { get; set; } = new();

    public override void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignature(Signature);

        Version = reader.Read<uint>();
        if(Version != 0x01060000)
        {
            throw new NotSupportedException();
        }

        reader.Skip(4); // Always 0?

        reader.ReadOffset(() => Parameters.AddFirst(reader.ReadObject<EffectParameter, Effect>(this)));
    }

    public override void Write(BinaryObjectWriter writer)
    {
        if(Version != 0x01060000)
        {
            throw new NotSupportedException();
        }

        writer.Write(Signature);
        writer.Write(Version);

        writer.Write(0); // Always 0?

        if(Parameters.First != null)
        {
            writer.WriteObjectOffset(Parameters.First.Value, this, 16);
        }
        else
        {
            writer.WriteOffsetValue(0);
        }
    }
}
