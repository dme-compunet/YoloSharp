namespace Compunet.YoloSharp.Services;

internal class MemoryAllocatorService : IMemoryAllocatorService
{
    #region ArrayMemoryPoolBuffer<T>

    private class ArrayMemoryPoolBuffer<T> : IMemoryOwner<T>
    {
        private readonly int _length;

        private T[]? _buffer;

        public Memory<T> Memory
        {
            get
            {
                ObjectDisposedException.ThrowIf(_buffer == null, this);
                return new Memory<T>(_buffer, 0, _length);
            }
        }

        public ArrayMemoryPoolBuffer(int length, bool clean)
        {
            var source = ArrayPool<T>.Shared.Rent(length);

            if (clean)
            {
                Array.Clear(source, 0, length);
            }

            _length = length;
            _buffer = source;
        }

        ~ArrayMemoryPoolBuffer() => Dispose();

        public void Dispose()
        {
            if (_buffer != null)
            {
                ArrayPool<T>.Shared.Return(_buffer);
                _buffer = null;
            }

            GC.SuppressFinalize(this);
        }
    }

    #endregion

    public IMemoryOwner<T> Allocate<T>(int length, bool clean = false)
    {
        return new ArrayMemoryPoolBuffer<T>(length, clean);
    }
}