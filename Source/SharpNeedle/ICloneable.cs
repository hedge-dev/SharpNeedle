namespace SharpNeedle;

public interface ICloneable<out T>
{
    T Clone();
}