namespace Compunet.YoloSharp.Services.Resolvers;

internal class PredictorServiceResolver : IDisposable
{
    private readonly YoloSession _yoloSession;
    private readonly YoloConfiguration _configuration;

    private readonly ServiceProvider _provider;
    private readonly Dictionary<YoloConfiguration, ServiceProvider> _providers = [];

    private bool _disposed;

    public PredictorServiceResolver(InferenceSession session, YoloConfiguration configuration)
    {
        _configuration = configuration;

        var metadata = YoloMetadata.Parse(session);
        var shapeInfo = new SessionIoShapeInfo(session, metadata);

        // Create default services
        var services = CreateDefaultServices(metadata);

        // Create yolo session
        _yoloSession = new YoloSession(metadata, session, shapeInfo);

        // Add yolo session
        services.AddSingleton(_yoloSession);
        services.AddSingleton(_configuration);

        // Build the service provider
        _provider = services.BuildServiceProvider();
    }

    public T Resolve<T>(YoloConfiguration? configuration = null) where T : notnull
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        if (configuration is null || _configuration.Equals(configuration))
        {
            return _provider.GetRequiredService<T>();
        }

        if (_providers.TryGetValue(configuration, out var p))
        {
            return p.GetRequiredService<T>();
        }
        else
        {
            var services = CreateDefaultServices(_yoloSession.Metadata);

            services.AddSingleton(_yoloSession);
            services.AddSingleton(configuration);

            var provider = services.BuildServiceProvider();

            _providers.Add(configuration, provider);

            return provider.GetRequiredService<T>();
        }
    }

    private static ServiceCollection CreateDefaultServices(YoloMetadata metadata)
    {
        var services = new ServiceCollection();

        if (metadata is YoloPoseMetadata pose)
        {
            services.AddSingleton(pose);
        }

        services
            .AddSingleton(metadata)
            .AddSingleton<ISessionRunnerService, SessionRunnerService>()
            .AddSingleton<IMemoryAllocatorService, MemoryAllocatorService>()
            .AddSingleton<IBoundingBoxTransformer, BoundingBoxTransformer>()
            .AddSingleton<IPixelsNormalizerService, PixelsNormalizerService>();

        var task = metadata.Task;
        var version = metadata.Architecture;

        var obb = task == YoloTask.Obb;

        AddNonMaxSuppression(services, obb);
        AddRawBoundingBoxParser(services, version, obb);

        switch (task)
        {
            case YoloTask.Pose:
                services.AddSingleton<IDecoder<Pose>, PoseDecoder>();
                break;

            case YoloTask.Detect:
                services.AddSingleton<IDecoder<Detection>, DetectionDecoder>();
                break;

            case YoloTask.Obb:
                services.AddSingleton<IDecoder<ObbDetection>, ObbDetectionDecoder>();
                break;

            case YoloTask.Segment:
                services.AddSingleton<IDecoder<Segmentation>, SegmentationDecoder>();
                break;

            case YoloTask.Classify:
                services.AddSingleton<IDecoder<Classification>, ClassificationDecoder>();
                break;
        }

        return services;
    }

    private static void AddNonMaxSuppression(ServiceCollection services, bool obb)
    {
        if (obb)
        {
            services.AddSingleton<INonMaxSuppressionService, ObbNonMaxSuppressionService>();
        }
        else
        {
            services.AddSingleton<INonMaxSuppressionService, NonMaxSuppressionService>();
        }
    }

    private static void AddRawBoundingBoxParser(ServiceCollection services, YoloArchitecture architecture, bool obb)
    {

        if (architecture == YoloArchitecture.AnchorFree)
        {
            if (obb)
            {
                services.AddSingleton<IBoundingBoxDecoder, AnchorFreeOrientedBoxDecoder>();
            }
            else
            {

                services.AddSingleton<IBoundingBoxDecoder, AnchorFreeBoxDecoder>();
            }
        }
        // anchor based
        else
        {
            if (obb)
            {
                services.AddSingleton<IBoundingBoxDecoder, AnchorBasedOrientedBoundingBoxDecoder>();
            }
            else
            {
                services.AddSingleton<IBoundingBoxDecoder, AnchorBasedBoundingBoxDecoder>();
            }
        }

    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _provider.Dispose();

        foreach (var provider in _providers.Values)
        {
            provider.Dispose();
        }

        _disposed = true;
    }
}