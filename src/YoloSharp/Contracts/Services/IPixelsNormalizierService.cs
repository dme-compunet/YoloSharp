namespace Compunet.YoloSharp.Contracts.Services;

internal interface IPixelsNormalizerService
{
    public void NormalizerPixelsToTensor(Image<Rgb24> image, MemoryTensor<float> tensor, Vector<int> padding);
}