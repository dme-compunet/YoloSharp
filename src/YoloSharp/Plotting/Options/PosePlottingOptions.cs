namespace Compunet.YoloSharp.Plotting;

public class PosePlottingOptions : DetectionPlottingOptions
{
    public static new PosePlottingOptions Default { get; } = new PosePlottingOptions();

    public ISkeleton Skeleton { get; set; }

    public float KeypointRadius { get; set; }

    public float KeypointLineThickness { get; set; }

    public float KeypointMinimumConfidence { get; set; }

    public PosePlottingOptions()
    {
        Skeleton = ISkeleton.Human;
        KeypointRadius = 3F;
        KeypointLineThickness = 1.5F;
        KeypointMinimumConfidence = .5F;
    }
}