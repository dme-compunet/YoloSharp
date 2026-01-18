namespace Compunet.YoloSharp.Services.Resolvers;

internal class PlottingServiceResolver
{
    private readonly ServiceProvider _provider;

    public static PlottingServiceResolver Default { get; } = new();

    public PlottingServiceResolver()
    {
        _provider = CreateDefaultServices()
                    .BuildServiceProvider();
    }

    public T Resolve<T>() where T : notnull
    {
        return _provider.GetRequiredService<T>();
    }

    private static ServiceCollection CreateDefaultServices()
    {
        var services = new ServiceCollection();

        services
            // Add drawers
            .AddSingleton<IBoxDrawer, BoxDrawer>()
            .AddSingleton<INameDrawer, NameDrawer>()
            .AddSingleton<IMaskDrawer, MaskDrawer>()
            .AddSingleton<ISkeletonDrawer, SkeletonDrawer>()

            // Add plotters
            .AddSingleton<IPlotter<Pose>, PosePlotter>()
            .AddSingleton<IPlotter<Detection>, DetectionPlotter>()
            .AddSingleton<IPlotter<ObbDetection>, ObbDetectionPlotter>()
            .AddSingleton<IPlotter<Segmentation>, SegmentationPlotter>()
            .AddSingleton<IPlotter<Classification>, ClassificationPlotter>()

            .AddSingleton<IImageContoursRecognizer, ImageContoursRecognizer>();

        return services;
    }
}