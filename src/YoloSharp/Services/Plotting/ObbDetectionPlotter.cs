namespace Compunet.YoloSharp.Services.Plotting;

internal class ObbDetectionPlotter(IBoxDrawer boxPlotter,
                                   INameDrawer namePlotter) : IPlotter<ObbDetection>
{
    public void Plot(YoloResult<ObbDetection> result, PlottingContext context)
    {
        foreach (var box in result)
        {
            var points = GetBoxPoints(box, out var namePosition);

            boxPlotter.DrawBox(box, points, context);
            namePlotter.DrawName(box, namePosition, false, context);
        }
    }

    private static PointF[] GetBoxPoints(ObbDetection detection, out PointF namePosition)
    {
        var points = detection.GetCornerPoints();
        var result = points.Select(point => new PointF(point.X, point.Y));

        namePosition = result.MinBy(point => point.Y);

        return [.. result];
    }
}