namespace Compunet.YoloSharp.Parsers.Base;

internal class AnchorFreeOrientedBoxDecoder(YoloMetadata metadata,
                                            YoloConfiguration configuration,
                                            IMemoryAllocatorService memoryAllocator,
                                            INonMaxSuppressionService nonMaxSuppression) : IBoundingBoxDecoder
{
    public RawBoundingBox[] Decode(MemoryTensor<float> tensor)
    {
        var strideP = tensor.Strides[metadata.PredictionAxis];
        var strideF = tensor.Strides[metadata.FeatureAxis];

        var predictionCount = tensor.Dimensions[metadata.PredictionAxis];

        using var boxes = memoryAllocator.Allocate<RawBoundingBox>(predictionCount);

        var boxesSpan = boxes.Memory.Span;
        var outputSpan = tensor.Span;

        var boxesIndex = 0;

        for (var boxIndex = 0; boxIndex < predictionCount; boxIndex++)
        {
            var boxOffset = boxIndex * strideP;

            var confidence = outputSpan[boxOffset + 4 * strideF];

            if (confidence <= configuration.Confidence)
            {
                continue;
            }

            var x = outputSpan[boxOffset + 0 * strideF];
            var y = outputSpan[boxOffset + 1 * strideF];
            var w = outputSpan[boxOffset + 2 * strideF];
            var h = outputSpan[boxOffset + 3 * strideF];

            var bounds = new RectangleF(x, y, w, h);

            if (bounds.Width == 0 || bounds.Height == 0)
            {
                continue;
            }

            var angle = NormalizeAngle(outputSpan[boxOffset + 6 * strideF]);

            var classIndex = (int)outputSpan[boxOffset + 5 * strideF];

            boxesSpan[boxesIndex++] = new RawBoundingBox
            {
                Index = boxIndex,
                NameIndex = classIndex,
                Confidence = confidence,
                Bounds = bounds,
                Angle = angle
            };
        }

        boxesSpan = boxesSpan[..boxesIndex];

        if (metadata.IsEndToEnd)
        {
            return [.. boxesSpan];
        }

        return nonMaxSuppression.Apply(boxesSpan, configuration.IoU);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float NormalizeAngle(float angle)
    {
        // Angle in [-pi/4,3/4 pi) -> [-pi/2,pi/2)
        if (angle >= MathF.PI && angle <= 0.75 * MathF.PI)
        {
            angle -= MathF.PI;
        }

        // Degrees
        return angle * 180f / MathF.PI;
    }
}
