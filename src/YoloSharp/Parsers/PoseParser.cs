namespace Compunet.YoloSharp.Parsers;

internal class PoseParser(YoloPoseMetadata metadata,
                          IImageAdjustmentService imageAdjustment,
                          IRawBoundingBoxParser rawBoundingBoxParser) : IParser<Pose>
{
    public Pose[] ProcessTensorToResult(IYoloRawOutput output, Size size)
    {
        var tensor = output.Output0;
        var adjustment = imageAdjustment.Calculate(size);

        var boxes = rawBoundingBoxParser.Parse(tensor);

        var shape = metadata.KeypointShape;
        var result = new Pose[boxes.Length];

        var tensorSpan = tensor.Span;
        var dataStride = tensor.Strides[1];

        for (var i = 0; i < boxes.Length; i++)
        {
            var box = boxes[i];
            var keypoints = new Keypoint[shape.Count];

            for (var index = 0; index < shape.Count; index++)
            {
                var offset = index * shape.Channels + 4 + metadata.Names.Length;

                var pointX = (int)((tensorSpan[offset * dataStride + box.Index] - adjustment.Padding.X) * adjustment.Ratio.X);
                var pointY = (int)((tensorSpan[(offset + 1) * dataStride + box.Index] - adjustment.Padding.Y) * adjustment.Ratio.Y);

                var pointConfidence = metadata.KeypointShape.Channels switch
                {
                    2 => 1f,
                    3 => tensorSpan[(offset + 2) * dataStride + box.Index],
                    _ => throw new InvalidOperationException("Unexpected keypoint shape")
                };

                keypoints[index] = new Keypoint
                {
                    Index = index,
                    Point = new Point(pointX, pointY),
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