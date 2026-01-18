namespace Compunet.YoloSharp.Parsers.Base;

internal class Yolo11RawBoundingBoxParser(YoloMetadata metadata,
                                          YoloConfiguration configuration,
                                          IMemoryAllocatorService memoryAllocator,
                                          INonMaxSuppressionService nonMaxSuppression) : IRawBoundingBoxParser
{
    public ImmutableArray<RawBoundingBox> Parse(MemoryTensor<float> tensor)
    {
        var boxStride = tensor.Strides[1];
        var boxesCount = tensor.Dimensions[2];
        var namesCount = metadata.Names.Length;

        using var boxes = memoryAllocator.Allocate<RawBoundingBox>(boxesCount);

        var boxesSpan = boxes.Memory.Span;
        var tensorSpan = tensor.Buffer.Span;

        var boxesIndex = 0;

        for (var boxIndex = 0; boxIndex < boxesCount; boxIndex++)
        {
            for (var nameIndex = 0; nameIndex < namesCount; nameIndex++)
            {
                var confidence = tensorSpan[(nameIndex + 4) * boxStride + boxIndex];

                if (confidence <= configuration.Confidence)
                {
                    continue;
                }

                ParseBox(tensorSpan, boxStride, boxIndex, out var bounds, out var angle);

                if (bounds.Width == 0 || bounds.Height == 0)
                {
                    continue;
                }

                boxesSpan[boxesIndex++] = new RawBoundingBox
                {
                    Index = boxIndex,
                    NameIndex = nameIndex,
                    Confidence = confidence,
                    Bounds = bounds,
                    Angle = angle
                };
            }
        }

        return nonMaxSuppression.Apply(boxesSpan[..boxesIndex], configuration.IoU);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    protected virtual void ParseBox(Span<float> tensor, int boxStride, int boxIndex, out RectangleF bounds, out float angle)
    {
        var x = tensor[0 + boxIndex];
        var y = tensor[1 * boxStride + boxIndex];
        var w = tensor[2 * boxStride + boxIndex];
        var h = tensor[3 * boxStride + boxIndex];

        bounds = new RectangleF(x - w / 2, y - h / 2, w, h);

        angle = float.NegativeZero;
    }
}