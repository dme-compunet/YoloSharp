namespace Compunet.YoloSharp.Memory;

internal class MemoryTensorOwner<T>(IMemoryOwner<T> owner, int[] dimensions) : IDisposable where T : unmanaged
{
    private MemoryTensor<T>? _tensor = new(owner.Memory, dimensions);

    public MemoryTensor<T> Tensor => _tensor
                                     ??
                                     throw new ObjectDisposedException(nameof(MemoryTensorOwner<T>));

    ~MemoryTensorOwner() => Dispose();

    public void Dispose()
    {
        if (_tensor != null)
        {
            _tensor = null;
            owner.Dispose();
        }

        GC.SuppressFinalize(this);
    }
}