namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

public readonly struct MeshSerializeContext
{
    public uint Version { get; }
    public bool IsMorphModel { get; }

    public MeshSerializeContext(uint version, bool isMorphModel)
    {
        Version = version;
        IsMorphModel = isMorphModel;
    }
}
