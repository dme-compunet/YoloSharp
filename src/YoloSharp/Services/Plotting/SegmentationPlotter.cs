namespace Compunet.YoloSharp.Services.Plotting;

internal class SegmentationPlotter(INameDrawer nameDrawer,
                                   IMaskDrawer maskDrawer) : IPlotter<Segmentation>
{
    public void Plot(YoloResult<Segmentation> result, PlottingContext context)
    {
        foreach (var box in result)
        {
            nameDrawer.DrawName(box, box.Bounds.Location, false, context);
            maskDrawer.DrawMask(box, context);
        }
    }
}