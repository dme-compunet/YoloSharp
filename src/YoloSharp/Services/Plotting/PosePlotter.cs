namespace Compunet.YoloSharp.Services.Plotting;

internal class PosePlotter(IBoxDrawer boxDrawer,
                           INameDrawer nameDrawer,
                           ISkeletonDrawer skeletonDrawer) : IPlotter<Pose>
{
    public void Plot(YoloResult<Pose> result, PlottingContext context)
    {
        foreach (var box in result)
        {
            boxDrawer.DrawBox(box, context);
            nameDrawer.DrawName(box, box.Bounds.Location, false, context);
            skeletonDrawer.DrawSkeleton(box, context);
        }
    }
}