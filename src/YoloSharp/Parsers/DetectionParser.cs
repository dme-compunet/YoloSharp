namespace Compunet.YoloSharp.Parsers;

internal class DetectionParser(YoloMetadata metadata,
                               IRawBoundingBoxParser rawBoundingBoxParser,
                               IImageAdjustmentService imageAdjustment) : IParser<Detection>
{
    public Detection[] ProcessTensorToResult(IYoloRawOutput output, Size size)
    {
        var boxes = rawBoundingBoxParser.Parse(output.Output0);

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