namespace Compunet.YoloSharp.Contracts.Services.Plotting;

internal interface IBoxDrawer
{
    public void DrawBox(Detection prediction, PlottingContext context);

    public void DrawBox(Detection prediction, PointF[] points, PlottingContext context);
}