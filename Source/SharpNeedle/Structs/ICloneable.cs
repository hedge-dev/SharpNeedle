namespace SharpNeedle.Structs;

public interface ICloneable<out T>
{
    T Clone();
}