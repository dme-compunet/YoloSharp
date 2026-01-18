namespace Compunet.YoloSharp;

public class YoloPredictor : IDisposable
{
    private readonly InferenceSession _session;
    private readonly PredictorServiceResolver _resolver;

    private bool _disposed;

    public YoloMetadata Metadata { get; }

    public YoloConfiguration Configuration { get; }

    #region Constractor

    public YoloPredictor(string path) : this(path, YoloPredictorOptions.Default) { }

    public YoloPredictor(byte[] model) : this(model, YoloPredictorOptions.Default) { }

    public YoloPredictor(string path, YoloPredictorOptions options) : this(options.CreateSession(path), options) { }
    
    public YoloPredictor(byte[] model, YoloPredictorOptions options) : this(options.CreateSession(model), options) { }

    private YoloPredictor(InferenceSession session, YoloPredictorOptions options)
    {
        _session = session;
        _resolver = new PredictorServiceResolver(_session, options.Configuration ?? YoloConfiguration.Default);
        Metadata = _resolver.Resolve<YoloMetadata>();
        Configuration = _resolver.Resolve<YoloConfiguration>();
    }

    #endregion

    #region Predict

    internal YoloResult<T> Predict<T>(Image<Rgb24> image, YoloConfiguration? configuration) where T : IYoloPrediction<T>
    {
        // Validate the model task
        ValidateTask<T>();

        // Resolve runner service
        var runner = _resolver.Resolve<ISessionRunnerService>();

        // Run the model (include pre-process)
        using var output = runner.PreprocessAndRun(image, out var timer);

        // Start postprocess timer
        timer.StartPostprocess();

        // Resolve the parser
        var parser = _resolver.Resolve<IParser<T>>(configuration);

        // Parse the tensor to result
        var result = parser.ProcessTensorToResult(output, image.Size);

        // Create YoloResult
        return new YoloResult<T>(result)
        {
            Speed = timer.Stop(),
            ImageSize = image.Size,
        };
    }

    #endregion

    internal T ResolveService<T>(YoloConfiguration? configuration) where T : notnull => _resolver.Resolve<T>(configuration);

    private void ValidateTask<T>() where T : IYoloPrediction<T>
    {
        YoloTask task;

        if (typeof(T) == typeof(Pose))
        {
            task = YoloTask.Pose;
        }
        else if (typeof(T) == typeof(Detection))
        {
            task = YoloTask.Detect;
        }
        else if (typeof(T) == typeof(ObbDetection))
        {
            task = YoloTask.Obb;
        }
        else if (typeof(T) == typeof(Segmentation))
        {
            task = YoloTask.Segment;
        }
        else if (typeof(T) == typeof(Classification))
        {
            task = YoloTask.Classify;
        }
        else
        {
            throw new InvalidOperationException();
        }

        var currentTask = Metadata.Task;

        if (currentTask != task)
        {
            throw new InvalidOperationException($"The loaded model does not support this task (expected: '{task.ToString().ToLower()}' actual: '{currentTask.ToString().ToLower()}')");
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _session.Dispose();
        _resolver.Dispose();
        _disposed = true;

        GC.SuppressFinalize(this);
    }
}