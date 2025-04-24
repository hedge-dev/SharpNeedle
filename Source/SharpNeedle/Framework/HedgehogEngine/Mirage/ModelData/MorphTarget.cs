namespace SharpNeedle.Framework.HedgehogEngine.Mirage.ModelData;

public class MorphTarget
{
    public string? Name { get; set; }
    public Vector3[] Positions { get; set; } = [];

    public override string ToString()
    {
        return Name ?? base.ToString()!;
    }
}
