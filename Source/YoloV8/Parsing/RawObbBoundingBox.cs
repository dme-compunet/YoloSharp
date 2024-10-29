﻿
namespace Compunet.YoloV8.Parsing;

internal readonly struct RawObbBoundingBox : IRawBoundingBox<RawObbBoundingBox>
{
    public required int Index { get; init; }

    public required int NameIndex { get; init; }

    public required RectangleF Bounds { get; init; }

    public required float Angle { get; init; }

    public required float Confidence { get; init; }

    public int CompareTo(RawObbBoundingBox other) => Confidence.CompareTo(other.Confidence);

    public static float CalculateIoU(ref RawObbBoundingBox box1, ref RawObbBoundingBox box2)
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

    public static RawObbBoundingBox Parse(ref RawParsingContext context, int index, int nameIndex, float confidence)
    {
        var nameCount = context.NameCount;
        var tensorSpan = context.Tensor.Span;
        var stride1 = context.Tensor.Strides[1];

        if (nameCount == 0)
        {
            throw new ArgumentException(nameof(nameCount));
        }

        var x = tensorSpan[index];
        var y = tensorSpan[1 * stride1 + index];
        var w = tensorSpan[2 * stride1 + index];
        var h = tensorSpan[3 * stride1 + index];

        // Radians
        var angle = tensorSpan[(4 + nameCount) * stride1 + index];

        // Angle in [-pi/4,3/4 pi) -> [-pi/2,pi/2)
        if (angle >= MathF.PI && angle <= 0.75 * MathF.PI)
        {
            angle -= MathF.PI;
        }

        // Degrees
        angle *= 180f / MathF.PI;

        var bounds = new RectangleF(x, y, w, h);

        return new RawObbBoundingBox
        {
            Index = index,
            NameIndex = nameIndex,
            Angle = angle,
            Bounds = bounds,
            Confidence = confidence
        };
    }
}