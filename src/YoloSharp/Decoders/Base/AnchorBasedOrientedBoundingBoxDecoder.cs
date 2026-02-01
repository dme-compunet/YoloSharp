namespace Compunet.YoloSharp.Decoders.Base;

internal class AnchorBasedOrientedBoundingBoxDecoder(YoloMetadata metadata,
                                                     YoloConfiguration configuration,
                                                     IMemoryAllocator memoryAllocator,
                                                     INonMaxSuppression nonMaxSuppression)
    : AnchorBasedBoundingBoxDecoder(metadata,
                                   configuration,
                                   memoryAllocator,
                                   nonMaxSuppression)
{
    private readonly int _namesCount = metadata.Names.Length;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected override void DecodeBox(Span<float> tensor, int boxStride, int boxIndex, out RectangleF bounds, out float angle)
    {
        var x = tensor[boxIndex];
        var y = tensor[1 * boxStride + boxIndex];
        var w = tensor[2 * boxStride + boxIndex];
        var h = tensor[3 * boxStride + boxIndex];

        bounds = new RectangleF(x, y, w, h);

        // Radians
        angle = tensor[(4 + _namesCount) * boxStride + boxIndex];

        // Angle in [-pi/4,3/4 pi) -> [-pi/2,pi/2)
        if (angle >= MathF.PI && angle <= 0.75 * MathF.PI)
        {
            angle -= MathF.PI;
        }

        // Degrees
        angle *= 180f / MathF.PI;
    }
}