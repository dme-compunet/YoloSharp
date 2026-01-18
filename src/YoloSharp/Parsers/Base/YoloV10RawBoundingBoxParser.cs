namespace Compunet.YoloSharp.Parsers.Base;

internal class YoloV10RawBoundingBoxParser(YoloConfiguration configuration,
                                           IMemoryAllocatorService memoryAllocator,
                                           INonMaxSuppressionService nonMaxSuppression) : IRawBoundingBoxParser
{
    public ImmutableArray<RawBoundingBox> Parse(MemoryTensor<float> tensor)
    {
        var boxStride = tensor.Strides[1];
        var dataStride = tensor.Strides[2];
        var boxesCount = tensor.Dimensions[1];

        using var boxes = memoryAllocator.Allocate<RawBoundingBox>(boxesCount);

        var boxesSpan = boxes.Memory.Span;
        var tensorSpan = tensor.Span;

        var boxesIndex = 0;

        for (var boxIndex = 0; boxIndex < boxesCount; boxIndex++)
        {
            var boxOffset = boxIndex * boxStride;

            var confidence = tensorSpan[boxOffset + 4 * dataStride];

            if (confidence <= configuration.Confidence)
            {
                continue;
            }

            var xMin = (int)tensorSpan[boxOffset + 0];
            var yMin = (int)tensorSpan[boxOffset + 1];
            var xMax = (int)tensorSpan[boxOffset + 2];
            var yMax = (int)tensorSpan[boxOffset + 3];

            var bounds = new RectangleF(xMin, yMin, xMax - xMin, yMax - yMin);

            if (bounds.Width == 0 || bounds.Height == 0)
            {
                continue;
            }

            var nameIndex = (int)tensorSpan[boxOffset + 5 * dataStride];

            boxesSpan[boxesIndex++] = new RawBoundingBox
            {
                Index = boxIndex,
                NameIndex = nameIndex,
                Confidence = confidence,
                Bounds = bounds
            };
        }

        return nonMaxSuppression.Apply(boxesSpan[..boxesIndex], configuration.IoU);
    }
}