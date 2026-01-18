namespace Compunet.YoloSharp.Plotting;

public static class PlottingAsyncExtensions
{
    public static async Task<Image> PlotImageAsync(this YoloResult<Pose> result, Image originImage, PosePlottingOptions? options = null)
    {
        return await Task.Run(() => result.PlotImage(originImage, options));
    }

    public static async Task<Image> PlotImageAsync(this YoloResult<Detection> result, Image originImage, DetectionPlottingOptions? options = null)
    {
        return await Task.Run(() => result.PlotImage(originImage, options));
    }

    public static async Task<Image> PlotImageAsync(this YoloResult<ObbDetection> result, Image originImage, DetectionPlottingOptions? options = null)
    {
        return await Task.Run(() => result.PlotImage(originImage, options));
    }

    public static async Task<Image> PlotImageAsync(this YoloResult<Segmentation> result, Image originImage, SegmentationPlottingOptions? options = null)
    {
        return await Task.Run(() => result.PlotImage(originImage, options));
    }

    public static async Task<Image> PlotImageAsync(this YoloResult<Classification> result, Image originImage, ClassificationPlottingOptions? options = null)
    {
        return await Task.Run(() => result.PlotImage(originImage, options));
    }
}