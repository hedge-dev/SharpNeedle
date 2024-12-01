namespace SharpNeedle.Structs;

public interface IIntersectable<in T>
{
    bool Intersects(T obj);
}