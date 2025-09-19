namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public struct KeyFrame
{
    public float Time { get; set; }
    public float Value { get; set; }
    public float ValueUpperBias { get; set; }
    public float ValueLowerBias { get; set; }
    public float SlopeL { get; set; }
    public float SlopeR { get; set; }
    public float SlopeLUpperBias { get; set; }
    public float SlopeLLowerBias { get; set; }
    public float SlopeRUpperBias { get; set; }
    public float SlopeRLowerBias { get; set; }
    public bool KeyBreak { get; set; }
}