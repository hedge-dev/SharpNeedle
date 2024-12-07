namespace SharpNeedle.Framework.HedgehogEngine.Needle.Archive;

using Amicitia.IO.Binary;
using SharpNeedle.Framework.HedgehogEngine.Mirage;
using SharpNeedle.Resource;
using System;

public class ModelBlock : SampleChunkResourceBlock<ModelBase>
{
    public const string ModelSignature = "NEDMDL";

    public override string Signature => ModelSignature;

    public bool FlipOffsets { get; set; }

    public ModelBlock() : base("model") { }


    protected override bool IsVersionValid(int version)
    {
        return version == 5;
    }

    protected override ModelBase CreateResourceInstance(string filename)
    {
        string extension = Path.GetExtension(filename).ToLower();

        if(extension == ".terrain-model")
        {
            return new TerrainModel();
        }
        else
        {
            return new Model();
        }
    }

}
