using Path = System.IO.Path;

namespace Compunet.YoloSharp.Plotting;

public static class PredictorPlottingExtensions
{
    #region TaskAndSave Sync

    public static YoloResult<Pose> PoseAndSave(this YoloPredictor predictor, string path, string? output = null, YoloConfiguration? configuration = null, PosePlottingOptions? options = null)
    {
        using var image = Image.Load(path);

        var result = predictor.Pose(image, configuration);

        using var plotted = result.PlotImage(image, options);

        output ??= CreateImageOutputPath(path, predictor.Metadata.Task);

        plotted.Save(output);

        return result;
    }

    public static YoloResult<Detection> DetectAndSave(this YoloPredictor predictor, string path, string? output = null, YoloConfiguration? configuration = null, DetectionPlottingOptions? options = null)
    {
        using var image = Image.Load(path);

        var result = predictor.Detect(image, configuration);

        using var plotted = result.PlotImage(image, options);

        output ??= CreateImageOutputPath(path, predictor.Metadata.Task);

        plotted.Save(output);

        return result;
    }

    public static YoloResult<ObbDetection> DetectObbAndSave(this YoloPredictor predictor, string path, string? output = null, YoloConfiguration? configuration = null, DetectionPlottingOptions? options = null)
    {
        using var image = Image.Load(path);

        var result = predictor.DetectObb(image, configuration);

        using var plotted = result.PlotImage(image, options);

        output ??= CreateImageOutputPath(path, predictor.Metadata.Task);

        plotted.Save(output);

        return result;
    }

    public static YoloResult<Segmentation> SegmentAndSave(this YoloPredictor predictor, string path, string? output = null, YoloConfiguration? configuration = null, SegmentationPlottingOptions? options = null)
    {
        using var image = Image.Load(path);

        var result = predictor.Segment(image, configuration);

        using var plotted = result.PlotImage(image, options);

        output ??= CreateImageOutputPath(path, predictor.Metadata.Task);

        plotted.Save(output);

        return result;
    }

    public static YoloResult<Classification> ClassifyAndSave(this YoloPredictor predictor, string path, string? output = null, YoloConfiguration? configuration = null, ClassificationPlottingOptions? options = null)
    {
        using var image = Image.Load(path);

        var result = predictor.Classify(image, configuration);

        using var plotted = result.PlotImage(image, options);

        output ??= CreateImageOutputPath(path, predictor.Metadata.Task);

        plotted.Save(output);

        return result;
    }

    #endregion

    #region TaskAndSaveAsync

    public static async Task<YoloResult<Pose>> PoseAndSaveAsync(this YoloPredictor predictor,
                                                                string path,
                                                                string? output = null,
                                                                YoloConfiguration? configuration = null,
                                                                PosePlottingOptions? options = null)
    {
        using var image = Image.Load(path);

        var result = await predictor.PoseAsync(image, configuration);

        using var plotted = await result.PlotImageAsync(image, options);

        output ??= CreateImageOutputPath(path, predictor.Metadata.Task);

        await plotted.SaveAsync(output);

        return result;
    }

    public static async Task<YoloResult<Detection>> DetectAndSaveAsync(this YoloPredictor predictor,
                                                                       string path,
                                                                       string? output = null,
                                                                       YoloConfiguration? configuration = null,
                                                                       DetectionPlottingOptions? options = null)
    {
        using var image = Image.Load(path);

        var result = await predictor.DetectAsync(image, configuration);

        using var plotted = await result.PlotImageAsync(image, options);

        output ??= CreateImageOutputPath(path, predictor.Metadata.Task);

        await plotted.SaveAsync(output);

        return result;
    }


    public static async Task<YoloResult<ObbDetection>> DetectObbAndSaveAsync(this YoloPredictor predictor,
                                                                             string path,
                                                                             string? output = null,
                                                                             YoloConfiguration? configuration = null,
                                                                             DetectionPlottingOptions? options = null)
    {
        using var image = Image.Load(path);

        var result = await predictor.DetectObbAsync(image, configuration);

        using var plotted = await result.PlotImageAsync(image, options);

        output ??= CreateImageOutputPath(path, predictor.Metadata.Task);

        await plotted.SaveAsync(output);

        return result;
    }

    public static async Task<YoloResult<Segmentation>> SegmentAndSaveAsync(this YoloPredictor predictor,
                                                                           string path,
                                                                           string? output = null,
                                                                           YoloConfiguration? configuration = null,
                                                                           SegmentationPlottingOptions? options = null)
    {
        using var image = Image.Load(path);

        var result = await predictor.SegmentAsync(image, configuration);

        using var plotted = await result.PlotImageAsync(image, options);

        output ??= CreateImageOutputPath(path, predictor.Metadata.Task);

        await plotted.SaveAsync(output);

        return result;
    }

    public static async Task<YoloResult<Classification>> ClassifyAndSaveAsync(this YoloPredictor predictor,
                                                                              string path,
                                                                              string? output = null,
                                                                              YoloConfiguration? configuration = null,
                                                                              ClassificationPlottingOptions? options = null)
    {
        using var image = Image.Load(path);

        var result = await predictor.ClassifyAsync(image, configuration);

        using var plotted = await result.PlotImageAsync(image, options);

        output ??= CreateImageOutputPath(path, predictor.Metadata.Task);

        await plotted.SaveAsync(output);

        return result;
    }

    #endregion

    #region PredictAndSave

    public static YoloResult PredictAndSave(this YoloPredictor predictor, string path, string? output = null, YoloConfiguration? configuration = null, PlottingOptions? options = null)
    {
        return predictor.Metadata.Task switch
        {
            YoloTask.Pose => PoseAndSave(predictor, path, output, configuration, options as PosePlottingOptions),
            YoloTask.Detect => DetectAndSave(predictor, path, output, configuration, options as DetectionPlottingOptions),
            YoloTask.Obb => DetectObbAndSave(predictor, path, output, configuration, options as DetectionPlottingOptions),
            YoloTask.Segment => SegmentAndSave(predictor, path, output, configuration, options as SegmentationPlottingOptions),
            YoloTask.Classify => ClassifyAndSave(predictor, path, output, configuration, options as ClassificationPlottingOptions),
            _ => throw new NotSupportedException("Unsupported YOLO task")
        };
    }

    public static async Task<YoloResult> PredictAndSaveAsync(this YoloPredictor predictor, string path, string? output = null, YoloConfiguration? configuration = null, PlottingOptions? options = null)
    {
        return predictor.Metadata.Task switch
        {
            YoloTask.Pose => await PoseAndSaveAsync(predictor, path, output, configuration, options as PosePlottingOptions),
            YoloTask.Detect => await DetectAndSaveAsync(predictor, path, output, configuration, options as DetectionPlottingOptions),
            YoloTask.Obb => await DetectObbAndSaveAsync(predictor, path, output, configuration, options as DetectionPlottingOptions),
            YoloTask.Segment => await SegmentAndSaveAsync(predictor, path, output, configuration, options as SegmentationPlottingOptions),
            YoloTask.Classify => await ClassifyAndSaveAsync(predictor, path, output, configuration, options as ClassificationPlottingOptions),
            _ => throw new NotSupportedException("Unsupported YOLO task")
        };
    }

    #endregion

    private static string CreateImageOutputPath(string path, YoloTask task)
    {
        var baseDirectory = Path.GetDirectoryName(path) ?? Environment.CurrentDirectory;

        var plotDirectory = Path.Combine(baseDirectory, task.ToString().ToLower());

        if (Directory.Exists(plotDirectory) == false)
        {
            Directory.CreateDirectory(plotDirectory);
        }

        var extn = Path.GetExtension(path);
        var name = Path.GetFileNameWithoutExtension(path);

        var index = 0;

        while (true)
        {
            var filename = index == 0 ? $"{name}{extn}" : $"{name}_{index}{extn}";
            var fullpath = Path.Combine(plotDirectory, filename);

            if (File.Exists(fullpath))
            {
                index++;
            }
            else
            {
                return fullpath;
            }
        }
    }
}