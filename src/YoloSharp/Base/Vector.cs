namespace Compunet.YoloSharp;

public readonly struct Vector<T>(T x, T y)
{
    public static Vector<T> Default { get; } = new();

    public T X => x;

    public T Y => y;

    public override string ToString() => $"X = {x}, Y = {y}";

    public static implicit operator Vector<T>(ValueTuple<T, T> tuple) => new(tuple.Item1, tuple.Item2);
}