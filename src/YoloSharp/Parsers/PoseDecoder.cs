namespace Compunet.YoloSharp.Parsers;

internal class PoseDecoder(YoloPoseMetadata metadata,
                           IImageAdjustmentService imageAdjustment,
                           IBoundingBoxDecoder rawBoundingBoxParser) : IDecoder<Pose>
{
    public Pose[] Decode(IYoloRawOutput output, Size size)
    {
        var tensor = output.Output0;
        var adjustment = imageAdjustment.Calculate(size);

        var boxes = rawBoundingBoxParser.Decode(tensor);

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

                var pointX = tensorSpan[baseOffset + offset * strideF] - adjustment.Padding.X;
                var pointY = tensorSpan[baseOffset + (offset + 1) * strideF] - adjustment.Padding.Y;

                pointX *= adjustment.Ratio.X;
                pointY *= adjustment.Ratio.Y;

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
                Bounds = imageAdjustment.Adjust(box.Bounds, adjustment),
                Confidence = box.Confidence,
            };
        }

        return result;
    }
}