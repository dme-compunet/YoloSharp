namespace Compunet.YoloSharp.Contracts.Services;

internal interface INonMaxSuppressionService
{
    public ImmutableArray<RawBoundingBox> Apply(Span<RawBoundingBox> boxes, float iouThreshold);
}