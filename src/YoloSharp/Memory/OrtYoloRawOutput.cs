namespace Compunet.YoloSharp.Memory;

internal class OrtYoloRawOutput : IYoloRawOutput
{
    private readonly IDisposable _disposable;

    public OrtYoloRawOutput(IDisposableReadOnlyCollection<DisposableNamedOnnxValue> result)
    {
        Output0 = CreateMemoryTensor(result[0]);

        if (result.Count > 1)
        {
            Output1 = CreateMemoryTensor(result[1]);
        }
;
        _disposable = result;
    }

    public MemoryTensor<float> Output0 { get; }

    public MemoryTensor<float>? Output1 { get; }

    public void Dispose() => _disposable.Dispose();

    private static MemoryTensor<float> CreateMemoryTensor(NamedOnnxValue value)
    {
        var tensor = value.AsTensor<float>() as DenseTensor<float>
                     ??
                     throw new InvalidOperationException("The ort result is not DenseTensor");

        return new MemoryTensor<float>(tensor.Buffer, [.. tensor.Dimensions]);
    }
}