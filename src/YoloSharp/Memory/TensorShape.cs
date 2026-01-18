namespace Compunet.YoloSharp.Memory;

internal readonly struct TensorShape
{
    public int Length { get; }

    public bool IsDynamic { get; }

    public int[] Dimensions { get; }

    public long[] Dimensions64 { get; }

    public TensorShape(int[] shape)
    {
        if (shape.Any(x => x < 0))
        {
            IsDynamic = true;
            Length = -1;
        }
        else
        {
            Length = GetSizeForShape(shape);
        }

        Dimensions = shape;
        Dimensions64 = [.. shape.Select(x => (long)x)];
    }

    private static int GetSizeForShape(ReadOnlySpan<int> shape)
    {
        var product = 1;

        for (var i = 0; i < shape.Length; i++)
        {
            var dimension = shape[i];

            if (dimension < 0)
            {
                throw new ArgumentOutOfRangeException($"Shape must not have negative elements: {dimension}");
            }

            product = checked(product * dimension);
        }

        return product;
    }
}
