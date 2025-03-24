namespace SharpNeedle.Framework.HedgehogEngine.Needle.Archive;

using SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

public class ModelBlock : SampleChunkResourceBlock<ModelBase>
{
    public const string ModelSignature = "NEDMDL";

    public override string Signature => ModelSignature;

    public ModelBlock() : base("model")
    {
        Version = 5;
    }

    protected override bool IsVersionValid(int version)
    {
        return version == 5;
    }

    protected override ModelBase CreateResourceInstance(string filename)
    {
        string extension = Path.GetExtension(filename).ToLowerInvariant();

        if (extension == ".terrain-model")
        {
            return new TerrainModel();
        }
        else
        {
            return new Model();
        }
    }

}
