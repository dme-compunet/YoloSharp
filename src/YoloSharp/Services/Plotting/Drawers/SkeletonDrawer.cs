namespace Compunet.YoloSharp.Services.Plotting;

internal class SkeletonDrawer : ISkeletonDrawer
{
    public void DrawSkeleton(Pose prediction, PlottingContext context)
    {
        var skeleton = context.Skeleton;

        // Draw lines
        for (var i = 0; i < skeleton.Connections.Length; i++)
        {
            var connection = skeleton.Connections[i];

            var first = prediction[connection.First];
            var second = prediction[connection.Second];

            if (first.Confidence < context.KeypointMinimumConfidence
                || second.Confidence < context.KeypointMinimumConfidence)
            {
                continue;
            }

            var points = new PointF[]
            {
                first.Point,
                second.Point,
            };

            var lineColor = skeleton.GetLineColor(i);

            context.Target.Mutate(x => x.DrawLine(lineColor, context.KeypointLineThickness, points));
        }

        // Draw keypoints
        foreach (var keypoint in prediction)
        {
            if (keypoint.Confidence < context.KeypointMinimumConfidence)
            {
                continue;
            }

            var ellipse = new EllipsePolygon(keypoint.Point, context.KeypointRadius);

            var keypointColor = skeleton.GetKeypointColor(keypoint.Index);

            context.Target.Mutate(x => x.Fill(keypointColor, ellipse));
        }
    }
}