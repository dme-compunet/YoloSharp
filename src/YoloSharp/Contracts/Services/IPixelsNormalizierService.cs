namespace Compunet.YoloSharp.Contracts.Services;

internal interface IPixelsNormalizer
{
    public void NormalizerPixelsToTensor(Image<Rgb24> image, MemoryTensor<float> tensor, Vector<int> padding);
}