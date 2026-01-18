namespace Compunet.YoloSharp.Services;

internal class ObbNonMaxSuppressionService : NonMaxSuppressionService
{
    protected override float CalculateIoU(RawBoundingBox box1, RawBoundingBox box2)
    {
        var rect1 = box1.Bounds;
        var rect2 = box2.Bounds;

        var area1 = rect1.Width * rect1.Height;

        if (area1 <= 0f)
        {
            return 0f;
        }

        var area2 = rect2.Width * rect2.Height;

        if (area2 <= 0f)
        {
            return 0f;
        }

        var vertices1 = box1.GetCornerPoints();
        var vertices2 = box2.GetCornerPoints();

        var path1 = new Path64(vertices1.Select(v => new Point64(v.X, v.Y)));
        var path2 = new Path64(vertices2.Select(v => new Point64(v.X, v.Y)));

        var subject = new Paths64([path1]);
        var clip = new Paths64([path2]);

        var intersection = Clipper.Intersect(subject, clip, FillRule.EvenOdd);
        var union = Clipper.Union(subject, clip, FillRule.EvenOdd);

        if (intersection.Count == 0 || union.Count == 0)
        {
            return 0f;
        }

        var intersectionArea = Clipper.Area(intersection[0]);
        var unionArea = Clipper.Area(union[0]);

        return (float)(intersectionArea / unionArea);
    }
}