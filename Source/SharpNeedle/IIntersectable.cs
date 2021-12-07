namespace SharpNeedle;

public interface IIntersectable<in T>
{
    bool Intersects(T obj);
}