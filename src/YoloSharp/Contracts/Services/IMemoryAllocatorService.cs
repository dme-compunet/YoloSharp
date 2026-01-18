namespace Compunet.YoloSharp.Contracts.Services;

internal interface IMemoryAllocatorService
{
    public IMemoryOwner<T> Allocate<T>(int length, bool clean = false);
}