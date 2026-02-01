namespace Compunet.YoloSharp.Contracts.Services;

internal interface IBoundingBoxDecoder
{
    public RawBoundingBox[] Decode(MemoryTensor<float> tensor);
}