namespace Compunet.YoloSharp.Contracts.Services.Plotting;

internal interface IMaskDrawer
{
    public void DrawMask(Segmentation prediction, PlottingContext context);
}