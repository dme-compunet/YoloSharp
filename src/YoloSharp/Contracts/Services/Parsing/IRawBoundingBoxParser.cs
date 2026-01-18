namespace Compunet.YoloSharp.Contracts.Services;

internal interface IRawBoundingBoxParser
{
    public ImmutableArray<RawBoundingBox> Parse(MemoryTensor<float> tensor);
}