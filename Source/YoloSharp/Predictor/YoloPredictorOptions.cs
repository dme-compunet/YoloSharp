namespace Compunet.YoloSharp;

public class YoloPredictorOptions
{
    public static YoloPredictorOptions Default { get; } = new();

#if USE_CUDA_DEFAULT
    public bool UseCuda { get; init; } = true;
#else 
    public bool UseCuda { get; init; }
#endif
    public int CudaDeviceId { get; init; }

    public SessionOptions? SessionOptions { get; init; }

    public YoloConfiguration? Configuration { get; init; }

    internal InferenceSession CreateSession(string path)
    {
        var sessionOptions = GetSessionOptions();
        
        if (sessionOptions != null)
        {
            return new InferenceSession(path, sessionOptions);
        }

        return new InferenceSession(path);
    }

    internal InferenceSession CreateSession(byte[] model)
    {
        var sessionOptions = GetSessionOptions();

        if (sessionOptions != null)
        {
            return new InferenceSession(model, sessionOptions);
        }

        return new InferenceSession(model);
    }

    private SessionOptions? GetSessionOptions()
    {
        if (UseCuda)
        {
            if (SessionOptions is not null)
            {
                throw new InvalidOperationException("'UseCuda' and 'SessionOptions' cannot be used together");
            }

            return SessionOptions.MakeSessionOptionWithCudaProvider(CudaDeviceId);
        }

        return SessionOptions;
    }
}
