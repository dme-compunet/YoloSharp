namespace Compunet.YoloSharp.Parsers.Base;

internal readonly struct RawBoundingBox : IComparable<RawBoundingBox>
{
    public required int Index { get; init; }

    public required int NameIndex { get; init; }

    public required float Confidence { get; init; }

    public required RectangleF Bounds { get; init; }

    public float Angle { get; init; }

    public int CompareTo(RawBoundingBox other) => Confidence.CompareTo(other.Confidence);
}