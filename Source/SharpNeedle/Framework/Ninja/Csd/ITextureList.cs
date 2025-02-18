namespace SharpNeedle.Framework.Ninja.Csd;

public interface ITextureList : IList<ITexture>, IChunk
{

}

public interface ITexture
{
    public string? Name { get; set; }
}