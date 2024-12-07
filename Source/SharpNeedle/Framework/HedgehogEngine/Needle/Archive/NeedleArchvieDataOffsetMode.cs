namespace SharpNeedle.Framework.HedgehogEngine.Needle.Archive;

public enum NeedleArchvieDataOffsetMode
{
    Default,

    /// <summary>
    /// Used in HE1 PC games; Offsets need to be flipped in their endianness
    /// </summary>
    Flipped,

    /// <summary>
    /// Used in HE2 games; Offsets depict a "distance" from themselve, rather
    /// than an offset to the start of the data, as well as being little endian
    /// </summary>
    SelfRelative
}
