namespace Compunet.YoloSharp.Parsers;

internal class ObbDetectionDecoder(YoloMetadata metadata,
                                   IBoundingBoxDecoder rawBoundingBoxParser,
                                   IImageAdjustmentService imageAdjustment) : IDecoder<ObbDetection>
{
    public ObbDetection[] Decode(IYoloRawOutput output, Size size)
    {
        var boxes = rawBoundingBoxParser.Decode(output.Output0);

        var adjustment = imageAdjustment.Calculate(size);

        var result = new ObbDetection[boxes.Length];

        for (var i = 0; i < boxes.Length; i++)
        {
            var box = boxes[i];

            result[i] = new ObbDetection
            {
                Name = metadata.Names[box.NameIndex],
                Angle = box.Angle,
                Bounds = imageAdjustment.Adjust(box.Bounds, adjustment),
                Confidence = box.Confidence,
            };
        }

        return result;
    }
}