namespace Compunet.YoloSharp.Plotting;

public class SegmentationPlottingOptions : DetectionPlottingOptions
{
    public static new SegmentationPlottingOptions Default { get; } = new SegmentationPlottingOptions();

    public float MaskAlpha { get; set; }

    public float ContoursThickness { get; set; }

    public float MaskMinimumConfidence { get; set; }

    public SegmentationPlottingOptions()
    {
        MaskAlpha = .4f;
        ContoursThickness = 1f;
        MaskMinimumConfidence = .5F;
    }
}