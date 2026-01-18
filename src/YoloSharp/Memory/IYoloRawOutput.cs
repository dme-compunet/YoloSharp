namespace Compunet.YoloSharp.Memory;

internal interface IYoloRawOutput : IDisposable
{
    public MemoryTensor<float> Output0 { get; }

    public MemoryTensor<float>? Output1 { get; }
}