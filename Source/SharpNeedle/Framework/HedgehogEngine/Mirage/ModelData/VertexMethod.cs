namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

public enum VertexMethod : byte
{
    Normal = 0,
    PartialU = 1,
    PartialV = 2,
    CrossUV = 3,
    UV = 4,
    Lookup = 5,
    LookupPreSampled = 6,
}
