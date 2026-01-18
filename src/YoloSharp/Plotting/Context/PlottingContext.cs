namespace Compunet.YoloSharp.Plotting;

internal class PlottingContext
{
    public Image<Rgba32> Target { get; }

    public TextOptions TextOptions { get; }

    public ColorPalette ColorPalette { get; }

    public float BorderThickness { get; }

    public Vector<float> NamePadding { get; }

    #region Pose

    public ISkeleton Skeleton { get; } = ISkeleton.Human;

    public float KeypointRadius { get; }

    public float KeypointLineThickness { get; }

    public float KeypointMinimumConfidence { get; }

    #endregion

    #region Segmentation

    public float MaskAlpha { get; set; }

    public float ContoursThickness { get; }

    public float MaskMinimumConfidence { get; }

    #endregion

    #region Classification

    public PointF Location { get; }

    #endregion

    public static PlottingContext Create<T>(PlottingOptions? options, Image<Rgba32> target, float gain) where T : IYoloPrediction<T>
    {
        if (options is null)
        {
            var predictionType = typeof(T);

            if (predictionType == typeof(Pose))
            {
                options = PosePlottingOptions.Default;
            }
            else if (predictionType == typeof(Segmentation))
            {
                options = SegmentationPlottingOptions.Default;
            }
            else if (predictionType == typeof(Detection))
            {
                options = DetectionPlottingOptions.Default;
            }
            else if (predictionType == typeof(ObbDetection))
            {
                options = DetectionPlottingOptions.Default;
            }
            else if (predictionType == typeof(Classification))
            {
                options = ClassificationPlottingOptions.Default;
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        if (options is PosePlottingOptions pose)
        {
            return new PlottingContext(pose, target, gain);
        }
        if (options is SegmentationPlottingOptions segmentation)
        {
            return new PlottingContext(segmentation, target, gain);
        }
        else if (options is DetectionPlottingOptions detection)
        {
            return new PlottingContext(detection, target, gain);
        }
        else if (options is ClassificationPlottingOptions classification)
        {
            return new PlottingContext(classification, target, gain);
        }
        else
        {
            throw new InvalidOperationException();
        }
    }

    #region Constructors

    private PlottingContext(PosePlottingOptions options, Image<Rgba32> target, float gain)
        : this(options as PlottingOptions, target, gain)
    {
        Skeleton = options.Skeleton;
        KeypointMinimumConfidence = options.KeypointMinimumConfidence;
        KeypointRadius = options.KeypointRadius * gain;
        KeypointLineThickness = options.KeypointLineThickness * gain;
    }

    private PlottingContext(DetectionPlottingOptions options, Image<Rgba32> target, float gain)
        : this(options as PlottingOptions, target, gain)
    { }

    private PlottingContext(SegmentationPlottingOptions options, Image<Rgba32> target, float gain)
        : this(options as PlottingOptions, target, gain)
    {
        MaskAlpha = options.MaskAlpha;
        ContoursThickness = options.ContoursThickness * gain;
        MaskMinimumConfidence = options.MaskMinimumConfidence;
    }

    private PlottingContext(ClassificationPlottingOptions options, Image<Rgba32> target, float gain)
        : this(options as PlottingOptions, target, gain)
    {
        Location = new PointF(options.Location.X * gain, options.Location.Y * gain);
    }

    private PlottingContext(PlottingOptions options, Image<Rgba32> target, float gain)
    {
        Target = target;
        TextOptions = CreateTextOptions(options, gain);
        ColorPalette = options.Palette;
        BorderThickness = options.BorderThickness * gain;
        NamePadding =
        (
            options.NamePadding.X * gain,
            options.NamePadding.Y * gain
        );
    }

    #endregion

    private static TextOptions CreateTextOptions(PlottingOptions options, float gain)
    {
        var font = options.FontFamily.CreateFont(options.FontSize * gain);

        return new TextOptions(font)
        {
            VerticalAlignment = VerticalAlignment.Center,
        };
    }
}