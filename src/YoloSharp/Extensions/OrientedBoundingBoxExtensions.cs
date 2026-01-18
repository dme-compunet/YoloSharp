namespace Compunet.YoloSharp.Extensions;

internal static class OrientedBoundingBoxExtensions
{
    public static Point[] GetCornerPoints(this ObbDetection obb)
    {
        return GetCornerPoints(obb.Bounds, obb.Angle);
    }

    public static Point[] GetCornerPoints(this RawBoundingBox box)
    {
        return GetCornerPoints(box.Bounds, box.Angle);
    }

    private static Point[] GetCornerPoints(RectangleF bounds, float angle)
    {
        var _angle = angle * MathF.PI / 180.0f; // Radians

        var b = MathF.Cos(_angle) * .5f;
        var a = MathF.Sin(_angle) * .5f;

        var x = bounds.X;
        var y = bounds.Y;
        var w = bounds.Width;
        var h = bounds.Height;

        var points = new Point[4];

        points[0].X = (int)MathF.Round(x - a * h - b * w, 0);
        points[0].Y = (int)MathF.Round(y + b * h - a * w, 0);

        points[1].X = (int)MathF.Round(x + a * h - b * w, 0);
        points[1].Y = (int)MathF.Round(y - b * h - a * w, 0);

        points[2].X = (int)MathF.Round(2f * x - points[0].X, 0);
        points[2].Y = (int)MathF.Round(2f * y - points[0].Y, 0);

        points[3].X = (int)MathF.Round(2f * x - points[1].X, 0);
        points[3].Y = (int)MathF.Round(2f * y - points[1].Y, 0);

        // Calculate the distances of each point from the origin (0, 0)
        var distance1 = Math.Sqrt(Math.Pow(points[0].X, 2) + Math.Pow(points[0].Y, 2));
        var distance2 = Math.Sqrt(Math.Pow(points[1].X, 2) + Math.Pow(points[1].Y, 2));

        // Rotate if necessary to ensure pt[0] is the top-left point
        if (distance2 < distance1)
        {
            var temp = points[0];
            points[0] = points[1];
            points[1] = points[2];
            points[2] = points[3];
            points[3] = temp;
        }

        return points;
    }
}