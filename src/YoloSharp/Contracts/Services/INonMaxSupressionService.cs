namespace Compunet.YoloSharp.Contracts.Services;

internal interface INonMaxSuppressionService
{
    public RawBoundingBox[] Apply(Span<RawBoundingBox> boxes, float iouThreshold);
}