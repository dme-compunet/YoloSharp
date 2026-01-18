namespace Compunet.YoloSharp.Plotting;

public readonly struct SkeletonConnection(int first, int second)
{
    public int First { get; } = first;

    public int Second { get; } = second;

    public static implicit operator SkeletonConnection(ValueTuple<int, int> tuple) => new(tuple.Item1, tuple.Item2);
}