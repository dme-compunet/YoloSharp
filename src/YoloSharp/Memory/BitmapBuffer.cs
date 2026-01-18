namespace Compunet.YoloSharp.Memory;

public class BitmapBuffer
{
    private readonly int _width;
    private readonly int _height;

    private readonly Memory<float> _buffer;

    public float this[int y, int x]
    {
        get => _buffer.Span[GetIndex(y, x)];
        set => _buffer.Span[GetIndex(y, x)] = value;
    }

    public int Width => _width;

    public int Height => _height;

    public BitmapBuffer(Memory<float> buffer, int width, int height)
    {
        if (buffer.Length != width * height)
        {
            throw new InvalidOperationException();
        }

        _width = width;
        _height = height;
        _buffer = buffer;
    }

    public BitmapBuffer(int width, int height)
    {
        _width = width;
        _height = height;
        _buffer = new Memory<float>(new float[height * width]);
    }

    public void Clear() => _buffer.Span.Clear();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIndex(int y, int x)
    {
        if (y < 0 || y >= _height)
        {
            throw new ArgumentOutOfRangeException(nameof(y));
        }

        if (x < 0 || x >= _width)
        {
            throw new ArgumentOutOfRangeException(nameof(x));
        }

        return (y * _width) + x;
    }
}