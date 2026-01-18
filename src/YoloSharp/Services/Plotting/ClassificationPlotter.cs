namespace Compunet.YoloSharp.Services.Plotting;

internal class ClassificationPlotter(INameDrawer nameDrawer) : IPlotter<Classification>
{
    public void Plot(YoloResult<Classification> result, PlottingContext context)
    {
        nameDrawer.DrawName(result.GetTopClass(), context.Location, true, context);
    }
}