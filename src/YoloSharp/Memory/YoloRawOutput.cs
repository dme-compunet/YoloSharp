namespace Compunet.YoloSharp.Memory;

internal class YoloRawOutput(MemoryTensorOwner<float> output0, MemoryTensorOwner<float>? output1) : IYoloRawOutput
{
    private bool _disposed;

    public MemoryTensor<float> Output0
    {
        get
        {
            EnsureNotDisposed();
            return output0.Tensor;
        }
    }

    public MemoryTensor<float>? Output1
    {
        get
        {
            EnsureNotDisposed();
            return output1?.Tensor;
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        output0.Dispose();
        output1?.Dispose();

        _disposed = true;
    }

    private void EnsureNotDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
    }
}