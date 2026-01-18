namespace Compunet.YoloSharp.Plotting;

public interface ISkeleton
{
    public static ISkeleton Human { get; } = new HumanSkeleton();

    SkeletonConnection[] Connections { get; }

    Color GetKeypointColor(int index);

    Color GetLineColor(int index);
}