namespace Compunet.YoloSharp.Parsers.Base;

internal class AnchorFreeBoxDecoder(YoloMetadata metadata,
                                            YoloConfiguration configuration,
                                            IMemoryAllocator memoryAllocator,
                                            INonMaxSuppression nonMaxSuppression) : IBoundingBoxDecoder
{
    public RawBoundingBox[] Decode(MemoryTensor<float> tensor)
    {
        var strideP = tensor.Strides[metadata.PredictionAxis];
        var strideF = tensor.Strides[metadata.FeatureAxis];

        var boxesCount = tensor.Dimensions[1];

        using var boxes = memoryAllocator.Allocate<RawBoundingBox>(boxesCount);

        var boxesSpan = boxes.Memory.Span;
        var tensorSpan = tensor.Span;

        var boxesIndex = 0;

        for (var boxIndex = 0; boxIndex < boxesCount; boxIndex++)
        {
            var boxOffset = boxIndex * strideP;

            var confidence = tensorSpan[boxOffset + 4 * strideF];

            if (confidence <= configuration.Confidence)
            {
                continue;
            }

            var xMin = (int)tensorSpan[boxOffset + 0 * strideF];
            var yMin = (int)tensorSpan[boxOffset + 1 * strideF];
            var xMax = (int)tensorSpan[boxOffset + 2 * strideF];
            var yMax = (int)tensorSpan[boxOffset + 3 * strideF];

            var bounds = new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin);

            if (bounds.Width == 0 || bounds.Height == 0)
            {
                continue;
            }

            var nameIndex = (int)tensorSpan[boxOffset + 5 * strideF];

            boxesSpan[boxesIndex++] = new RawBoundingBox
            {
                Index = boxIndex,
                NameIndex = nameIndex,
                Confidence = confidence,
                Bounds = bounds
            };
        }

        boxesSpan = boxesSpan[..boxesIndex];

        if (metadata.IsEndToEnd)
        {
            return [.. boxesSpan];
        }

        return nonMaxSuppression.Apply(boxesSpan, configuration.IoU);
    }
}