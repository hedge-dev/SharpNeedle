using SharpNeedle.BINA;

namespace SharpNeedle.Glitter;

[NeedleResource("glitter/effect", @"\.(gle|effect)$")]
public class Effect : BinaryResource
{
    public new static readonly uint Signature = BinaryHelper.MakeSignature<uint>("FIRG");
    public uint Version { get; set; }
    public LinkedList<EffectParameter> Parameters { get; set; } = new();
    
    public override void Read(BinaryObjectReader reader)
    {
        reader.EnsureSignature(Signature);

        Version = reader.Read<uint>();

        reader.Skip(4); // Always 0?

        reader.ReadOffset(() =>
        {
            Parameters.AddFirst(reader.ReadObject<EffectParameter, Effect>(this));
        });
    }

    public override void Write(BinaryObjectWriter writer)
    {
        writer.Write(Signature);
        writer.Write(Version);

        writer.Write(0); // Always 0?

        writer.WriteObjectOffset(Parameters.First.Value, this, 16);
    }
}
