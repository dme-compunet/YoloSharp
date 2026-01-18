namespace Compunet.YoloSharp.Plotting;

public abstract class PlottingOptions
{
    public FontFamily FontFamily { get; set; }

    public float FontSize { get; set; }

    public Vector<float> NamePadding { get; set; }

    public float BorderThickness { get; set; }

    public ColorPalette Palette { get; set; }

    public PlottingOptions()
    {
        FontFamily = GetDefaultFontFamily();
        FontSize = 12f;
        BorderThickness = 1;
        NamePadding = (6, 4);
        Palette = ColorPalette.Default;
    }

    private static FontFamily GetDefaultFontFamily()
    {
        if (OperatingSystem.IsWindows() && SystemFonts.TryGet("Microsoft YaHei", out var family))
        {
            return family;
        }

        if (OperatingSystem.IsAndroid() && SystemFonts.TryGet("Robot", out family))
        {
            return family;
        }

        return SystemFonts.Families.FirstOrDefault();
    }
}