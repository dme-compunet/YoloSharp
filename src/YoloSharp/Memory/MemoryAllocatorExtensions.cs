namespace Compunet.YoloSharp.Memory;

internal static class MemoryAllocatorExtensions
{
    public static MemoryTensorOwner<T> AllocateTensor<T>(this IMemoryAllocatorService allocator, TensorShape shape, bool clean = false)
        where T : unmanaged
    {
        var memory = allocator.Allocate<T>(shape.Length, clean);

        return new MemoryTensorOwner<T>(memory, shape.Dimensions);
    }
}