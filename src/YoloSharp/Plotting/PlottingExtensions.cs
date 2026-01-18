namespace Compunet.YoloSharp.Plotting;

public static class PlottingExtensions
{
    private static readonly PlottingServiceResolver _resolver = PlottingServiceResolver.Default;

    public static Image PlotImage<T>(this YoloResult<T> result, Image image, PlottingOptions? options = null) where T : IYoloPrediction<T>
    {
        // Get image as Rgba32 pixel format
        var target = image.CloneAs<Rgba32>();

        // Apply auto orient to target image
        target.AutoOrient();

        // Validate that target size equals to result size
        if (result.ImageSize != target.Size)
        {
            throw new InvalidOperationException("YOLO prediction result size is not equals to the target image size");
        }

        // Calculate required scale factory
        var gain = Math.Max(target.Width, target.Height) / 640f;

        var context = PlottingContext.Create<T>(options, target, gain);

        // Resolve plotter
        var plotter = _resolver.Resolve<IPlotter<T>>();

        // Plot the result
        plotter.Plot(result, context);

        // Return the plotted image
        return target;
    }
}