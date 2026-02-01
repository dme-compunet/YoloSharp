namespace Compunet.YoloSharp.Decoders;

internal class DetectionDecoder(YoloMetadata metadata,
                                IBoundingBoxDecoder boxDecoder,
                                IBoundingBoxTransformer transformer) : IDecoder<Detection>
{
    public Detection[] Decode(IYoloRawOutput output, Size size)
    {
        var boxes = boxDecoder.Decode(output.Output0);

        var transform = transformer.Compute(size);

        var result = new Detection[boxes.Length];

        for (var i = 0; i < boxes.Length; i++)
        {
            var box = boxes[i];

            result[i] = new Detection
            {
                Name = metadata.Names[box.NameIndex],
                Bounds = transformer.Apply(box.Bounds, transform),
                Confidence = box.Confidence,
            };
        }

        return result;
    }
}