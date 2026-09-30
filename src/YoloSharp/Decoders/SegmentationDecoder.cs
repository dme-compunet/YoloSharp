namespace Compunet.YoloSharp.Decoders;

internal class SegmentationDecoder(YoloMetadata metadata,
                                   IBoundingBoxDecoder boxDecoder,
                                   IBoundingBoxTransformer transformer,
                                   IMemoryAllocator memoryAllocator) : IDecoder<Segmentation>
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

        // PERF: the mask reconstruction below originally iterated the **channel dimension innermost**
        // (`for y: for x: for i: value += output1[0, i, y, x] * w[i]`).
        // output1 is laid out [batch][channel][y][x], so walking `i` with (y, x) fixed strides
        // maskHeight * maskWidth * 4 bytes (~160 KB for a 200x200 prototype) — i.e. nearly every
        // multiply-add was a cache miss. A desktop CPU with a large L3 absorbs this; a phone
        // (1-4 MB of last-level cache) does not: the same managed code measured ~50 ms on Windows
        // vs ~860 ms on Android (17x) for 16 objects, ~20.5M cache-missing accesses.
        // Fix: move the channel dimension to the OUTER loop so the innermost loop walks x
        // contiguously, hoist the strides, and work on the underlying Span instead of the
        // BitmapBuffer indexer (which evaluates two ArgumentOutOfRange branches per access).
        var output1Span = output1.Span;
        var strideChannel = output1.Strides[1];
        var strideRow = output1.Strides[2];
        var strideCol = output1.Strides[3];
        var maskSpan = rawMaskBuffer.Memory.Span;

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

            // Equivalent to mask.Clear() (same buffer), but limited to the region actually used.
            maskSpan.Slice(0, maskWidth * maskHeight).Clear();

            // Channel dimension outermost: the inner loop now walks x contiguously (see above).
            for (var i = 0; i < maskChannelCount; i++)
            {
                var weight = weightsSpan[i];
                var channelBase = (i * strideChannel)
                                  + (maskPaddingY * strideRow)
                                  + (maskPaddingX * strideCol);

                for (var y = 0; y < maskHeight; y++)
                {
                    var inRow = channelBase + (y * strideRow);
                    var outRow = y * maskWidth;

                    for (var x = 0; x < maskWidth; x++)
                    {
                        maskSpan[outRow + x] += output1Span[inRow + (x * strideCol)] * weight;
                    }
                }
            }

            // Sigmoid as its own pass: same number of MathF.Exp calls, but over contiguous
            // memory and no longer interleaved with the accumulation loop above.
            for (var k = 0; k < maskWidth * maskHeight; k++)
            {
                maskSpan[k] = Sigmoid(maskSpan[k]);
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
        // PERF: same class of problem as the mask reconstruction above — per-pixel access went
        // through the BitmapBuffer indexer, which evaluates two ArgumentOutOfRange branches on
        // every read/write. Object boxes are often several hundred pixels per side, so this added
        // up to tens of millions of branches per image.
        // Now reads/writes go through Span with precomputed row offsets. The interpolation
        // formula — including the exact multiply/divide order of `(x + position.X) * (w - 1) / (size - 1)`
        // — is copied verbatim, so the output is bit-for-bit identical to the previous implementation.
        var src = source.AsReadOnlySpan();
        var dst = target.AsSpan();

        var sourceWidth = source.Width;
        var sourceHeight = source.Height;
        var targetWidth = target.Width;

        for (var y = 0; y < target.Height; y++)
        {
            var outRow = y * targetWidth;

            // Calculate source coordinates
            var sourceY = (float)(y + position.Y) * (sourceHeight - 1) / (size.Height - 1);

            // Check if source coordinates are out of bounds
            if (sourceY < 0 || sourceY >= sourceHeight)
            {
                dst.Slice(outRow, targetWidth).Clear();
                continue;
            }

            // Ensure coordinates are within valid range for interpolation
            var y0 = Math.Max(0, Math.Min((int)sourceY, sourceHeight - 2));
            var y1 = y0 + 1;

            // Calculate interpolation factors
            var yLerp = sourceY - y0;

            var row0 = y0 * sourceWidth;
            var row1 = y1 * sourceWidth;

            for (var x = 0; x < targetWidth; x++)
            {
                // Calculate source coordinates
                var sourceX = (float)(x + position.X) * (sourceWidth - 1) / (size.Width - 1);

                // Check if source coordinates are out of bounds
                if (sourceX < 0 || sourceX >= sourceWidth)
                {
                    dst[outRow + x] = 0f;
                    continue;
                }

                // Ensure coordinates are within valid range for interpolation
                var x0 = Math.Max(0, Math.Min((int)sourceX, sourceWidth - 2));
                var x1 = x0 + 1;

                // Calculate interpolation factors
                var xLerp = sourceX - x0;

                // Perform bilinear interpolation
                var top = Lerp(src[row0 + x0], src[row0 + x1], xLerp);
                var bottom = Lerp(src[row1 + x0], src[row1 + x1], xLerp);

                dst[outRow + x] = Lerp(top, bottom, yLerp);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Lerp(float a, float b, float t) => a + (b - a) * t;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static float Sigmoid(float value) => 1 / (1 + MathF.Exp(-value));
}