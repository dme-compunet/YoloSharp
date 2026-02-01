namespace Compunet.YoloSharp.Parsers;

internal class DetectionDecoder(YoloMetadata metadata,
                                IBoundingBoxDecoder rawBoundingBoxParser,
                                IImageAdjustmentService imageAdjustment) : IDecoder<Detection>
{
    public Detection[] Decode(IYoloRawOutput output, Size size)
    {
        var boxes = rawBoundingBoxParser.Decode(output.Output0);

        var adjustment = imageAdjustment.Calculate(size);

        var result = new Detection[boxes.Length];

        for (var i = 0; i < boxes.Length; i++)
        {
            var box = boxes[i];

            result[i] = new Detection
            {
                Name = metadata.Names[box.NameIndex],
                Bounds = imageAdjustment.Adjust(box.Bounds, adjustment),
                Confidence = box.Confidence,
            };
        }

        return result;
    }
}