namespace Compunet.YoloSharp.Parsers;

internal class SegmentationDecoder(YoloMetadata metadata,
                                   IBoundingBoxDecoder boxDecoder,
                                   IBoundingBoxTransformer transformer,
                                   IMemoryAllocatorService memoryAllocator) : IDecoder<Segmentation>
{
    public Segmentation[] Decode(IYoloRawOutput output, Size size)
    {
        var transform = transformer.Compute(size);

        var output0 = output.Output0;
        var output1 = output.Output1
                      ??
                      throw new InvalidOperationException();

        var maskWidth = output1.Dimensions[3];
        var maskHeight = output1.Dimensions[2];
        var maskChannelCount = output1.Dimensions[1];

        var maskPaddingX = transform.Padding.X * maskWidth / metadata.ImageSize.Width;
        var maskPaddingY = transform.Padding.Y * maskHeight / metadata.ImageSize.Height;

        maskWidth -= maskPaddingX * 2;
        maskHeight -= maskPaddingY * 2;

        using var rawMaskBuffer = memoryAllocator.Allocate<float>(maskWidth * maskHeight);
        using var weightsBuffer = memoryAllocator.Allocate<float>(maskChannelCount);

        var weightsSpan = weightsBuffer.Memory.Span;
        var mask = new BitmapBuffer(rawMaskBuffer.Memory, maskWidth, maskHeight);

        var offsetToWeights = metadata.AttributeOffset;

        var strideF = output0.Strides[metadata.FeatureAxis];
        var strideP = output0.Strides[metadata.PredictionAxis];

        var output0Span = output0.Span;

        var boxes = boxDecoder.Decode(output0);

        var result = new Segmentation[boxes.Length];

        for (var index = 0; index < boxes.Length; index++)
        {
            var box = boxes[index];
            var boxIndex = box.Index;

            var bounds = transformer.Apply(box.Bounds, transform);

            // Collect the weights for this box
            for (var i = 0; i < maskChannelCount; i++)
            {
                weightsSpan[i] = output0Span[boxIndex * strideP + (offsetToWeights + i) * strideF];
            }

            mask.Clear();

            for (var y = 0; y < mask.Height; y++)
            {
                for (var x = 0; x < mask.Width; x++)
                {
                    var value = 0f;

                    for (var i = 0; i < maskChannelCount; i++)
                    {
                        value += output1[0, i, y + maskPaddingY, x + maskPaddingX] * weightsSpan[i];
                    }

                    mask[y, x] = Sigmoid(value);
                }
            }

            var resizedMask = new BitmapBuffer(bounds.Width, bounds.Height);

            ResizeToTarget(mask, resizedMask, bounds.Location, size);

            result[index] = new Segmentation
            {
                Mask = resizedMask,
                Name = metadata.Names[box.NameIndex],
                Bounds = bounds,
                Confidence = box.Confidence,
            };
        }

        return result;
    }

    private static void ResizeToTarget(BitmapBuffer source, BitmapBuffer target, Point position, Size size)
    {
        for (var y = 0; y < target.Height; y++)
        {
            for (var x = 0; x < target.Width; x++)
            {
                // Calculate source coordinates
                var sourceX = (float)(x + position.X) * (source.Width - 1) / (size.Width - 1);
                var sourceY = (float)(y + position.Y) * (source.Height - 1) / (size.Height - 1);

                // Check if source coordinates are out of bounds
                if (sourceY < 0 || sourceY >= source.Height ||
                    sourceX < 0 || sourceX >= source.Width)
                {
                    target[y, x] = 0f;
                    continue;
                }

                // Ensure coordinates are within valid range for interpolation
                var x0 = Math.Max(0, Math.Min((int)sourceX, source.Width - 2));
                var y0 = Math.Max(0, Math.Min((int)sourceY, source.Height - 2));

                var x1 = x0 + 1;
                var y1 = y0 + 1;

                // Calculate interpolation factors
                var xLerp = sourceX - x0;
                var yLerp = sourceY - y0;

                // Perform bilinear interpolation
                var top = Lerp(source[y0, x0], source[y0, x1], xLerp);
                var bottom = Lerp(source[y1, x0], source[y1, x1], xLerp);

                target[y, x] = Lerp(top, bottom, yLerp);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Lerp(float a, float b, float t) => a + (b - a) * t;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Sigmoid(float value) => 1 / (1 + MathF.Exp(-value));
}