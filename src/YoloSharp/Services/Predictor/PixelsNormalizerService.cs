namespace Compunet.YoloSharp.Services;

internal class PixelsNormalizerService : IPixelsNormalizerService
{
    public void NormalizerPixelsToTensor(Image<Rgb24> image, MemoryTensor<float> tensor, Vector<int> padding)
    {
        // Verify tensor dimensions
        if (image.Height + (padding.Y * 2) != tensor.Dimensions[2] && image.Width + (padding.X * 2) != tensor.Dimensions[3])
        {
            throw new InvalidOperationException("The image size and target tensor dimensions is not match");
        }

        // Process core
        ProcessToTensorCore(image, tensor, padding);
    }

    private static void ProcessToTensorCore(Image<Rgb24> image, MemoryTensor<float> tensor, Vector<int> padding)
    {
        var width = image.Width;
        var height = image.Height;

        // Pre-calculate strides for performance
        var strideY = tensor.Strides[2];
        var strideX = tensor.Strides[3];
        var strideR = tensor.Strides[1] * 0;
        var strideG = tensor.Strides[1] * 1;
        var strideB = tensor.Strides[1] * 2;

        // Get a span of the whole tensor for fast access
        var tensorSpan = tensor.Span;

        // Try get continuous memory block of the entire image data
        if (image.DangerousTryGetSinglePixelMemory(out var memory))
        {
            var pixels = memory.Span;
            var length = height * width;

            for (var index = 0; index < length; index++)
            {
                var x = index % width;
                var y = index / width;

                var tensorIndex = strideR + strideY * (y + padding.Y) + strideX * (x + padding.X);

                var pixel = pixels[index];

                WritePixel(tensorSpan, tensorIndex, pixel, strideR, strideG, strideB);
            }
        }
        else
        {
            for (var y = 0; y < height; y++)
            {
                var rowSpan = image.DangerousGetPixelRowMemory(y).Span;
                var tensorYIndex = strideR + strideY * (y + padding.Y);

                for (var x = 0; x < width; x++)
                {
                    var tensorIndex = tensorYIndex + strideX * (x + padding.X);
                    var pixel = rowSpan[x];

                    WritePixel(tensorSpan, tensorIndex, pixel, strideR, strideG, strideB);
                }
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void WritePixel(Span<float> target, int index, Rgb24 pixel, int strideBatchR, int strideBatchG, int strideBatchB)
    {
        target[index] = pixel.R / 255f;
        target[index + strideBatchG - strideBatchR] = pixel.G / 255f;
        target[index + strideBatchB - strideBatchR] = pixel.B / 255f;
    }
}