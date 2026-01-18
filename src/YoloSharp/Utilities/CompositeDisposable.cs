namespace Compunet.YoloSharp.Utilities;

internal class CompositeDisposable(IEnumerable<IDisposable> disposables) : IDisposable
{
    private bool _disposed;

    ~CompositeDisposable() => Dispose();

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        foreach (var disposable in disposables)
        {
            disposable.Dispose();
        }

        _disposed = true;

        GC.SuppressFinalize(this);
    }
}