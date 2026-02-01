namespace Compunet.YoloSharp.Decoders;

internal class ObbDetectionDecoder(YoloMetadata metadata,
                                   IBoundingBoxDecoder boxDecoder,
                                   IBoundingBoxTransformer transformer) : IDecoder<ObbDetection>
{
    public ObbDetection[] Decode(IYoloRawOutput output, Size size)
    {
        var boxes = boxDecoder.Decode(output.Output0);

        var transform = transformer.Compute(size);

        var result = new ObbDetection[boxes.Length];

        for (var i = 0; i < boxes.Length; i++)
        {
            var box = boxes[i];

            result[i] = new ObbDetection
            {
                Name = metadata.Names[box.NameIndex],
                Angle = box.Angle,
                Bounds = transformer.Apply(box.Bounds, transform),
                Confidence = box.Confidence,
            };
        }

        return result;
    }
}