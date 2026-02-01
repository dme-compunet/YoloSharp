namespace Compunet.YoloSharp.Parsers;

internal class PoseDecoder(YoloPoseMetadata metadata,
                           IBoundingBoxDecoder boxDecoder,
                           IBoundingBoxTransformer transformer) : IDecoder<Pose>
{
    public Pose[] Decode(IYoloRawOutput output, Size size)
    {
        var tensor = output.Output0;
        var transform = transformer.Compute(size);

        var boxes = boxDecoder.Decode(tensor);

        var shape = metadata.KeypointShape;
        var result = new Pose[boxes.Length];

        var tensorSpan = tensor.Span;

        var strideF = tensor.Strides[metadata.FeatureAxis];
        var strideP = tensor.Strides[metadata.PredictionAxis];

        var offsetToKeypoint = metadata.AttributeOffset;

        for (var i = 0; i < boxes.Length; i++)
        {
            var box = boxes[i];
            var keypoints = new Keypoint[shape.Count];

            var baseOffset = box.Index * strideP;

            for (var index = 0; index < shape.Count; index++)
            {
                var offset = index * shape.Channels + offsetToKeypoint;

                var pointX = tensorSpan[baseOffset + offset * strideF] - transform.Padding.X;
                var pointY = tensorSpan[baseOffset + (offset + 1) * strideF] - transform.Padding.Y;

                pointX *= transform.Ratio.X;
                pointY *= transform.Ratio.Y;

                var pointConfidence = metadata.KeypointShape.Channels switch
                {
                    2 => 1f,
                    3 => tensorSpan[baseOffset + (offset + 2) * strideF],
                    _ => throw new InvalidOperationException("Unexpected keypoint shape")
                };

                keypoints[index] = new Keypoint
                {
                    Index = index,
                    Point = new Point((int)pointX, (int)pointY),
                    Confidence = pointConfidence
                };
            }

            result[i] = new Pose(keypoints)
            {
                Name = metadata.Names[box.NameIndex],
                Bounds = transformer.Apply(box.Bounds, transform),
                Confidence = box.Confidence,
            };
        }

        return result;
    }
}