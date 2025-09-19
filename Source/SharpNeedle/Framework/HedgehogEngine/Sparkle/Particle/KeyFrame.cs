namespace SharpNeedle.Framework.HedgehogEngine.Sparkle;

public struct KeyFrame
{
    public float Time;
    public float Value;
    public float ValueUpperBias;
    public float ValueLowerBias;
    public float SlopeL;
    public float SlopeR;
    public float SlopeLUpperBias;
    public float SlopeLLowerBias;
    public float SlopeRUpperBias;
    public float SlopeRLowerBias;
    public bool KeyBreak;
}