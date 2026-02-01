namespace Compunet.YoloSharp.Contracts.Services;

internal interface IMemoryAllocator
{
    public IMemoryOwner<T> Allocate<T>(int length, bool clean = false);
}