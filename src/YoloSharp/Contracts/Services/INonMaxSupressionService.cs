namespace Compunet.YoloSharp.Contracts.Services;

internal interface INonMaxSuppression
{
    public RawBoundingBox[] Apply(Span<RawBoundingBox> boxes, float iouThreshold);
}