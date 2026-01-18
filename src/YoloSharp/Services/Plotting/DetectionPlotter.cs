namespace Compunet.YoloSharp.Services.Plotting;

internal class DetectionPlotter(IBoxDrawer boxPlotter,
                                INameDrawer namePlotter) : IPlotter<Detection>
{
    public void Plot(YoloResult<Detection> result, PlottingContext context)
    {
        foreach (var box in result)
        {
            boxPlotter.DrawBox(box, context);
            namePlotter.DrawName(box, box.Bounds.Location, false, context);
        }
    }
}