namespace Compunet.YoloSharp.Plotting;

public class ClassificationPlottingOptions : PlottingOptions
{
    public static ClassificationPlottingOptions Default { get; } = new ClassificationPlottingOptions();

    public Point Location { get; set; }

    public ClassificationPlottingOptions()
    {
        FontSize = 16;
        NamePadding = (12, 6);
        Location = new Point(30, 30);
    }
}