namespace Compunet.YoloSharp;

public static class YoloPredictorAsyncExtensions
{
    private static readonly DecoderOptions _skipMetadataOptions = new()
    {
        SkipMetadata = true,
    };

    #region Predict Image From Path

    public static Task<YoloResult<Pose>> PoseAsync(this YoloPredictor predictor, string path, YoloConfiguration? configuration = null)
        => PoseAsync(predictor, LoadImage(path, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<Detection>> DetectAsync(this YoloPredictor predictor, string path, YoloConfiguration? configuration = null)
        => DetectAsync(predictor, LoadImage(path, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<ObbDetection>> DetectObbAsync(this YoloPredictor predictor, string path, YoloConfiguration? configuration = null)
        => DetectObbAsync(predictor, LoadImage(path, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<Segmentation>> SegmentAsync(this YoloPredictor predictor, string path, YoloConfiguration? configuration = null)
        => SegmentAsync(predictor, LoadImage(path, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<Classification>> ClassifyAsync(this YoloPredictor predictor, string path, YoloConfiguration? configuration = null)
        => ClassifyAsync(predictor, LoadImage(path, configuration ?? predictor.Configuration), configuration);

    #endregion

    #region Predict Image From Stream

    public static Task<YoloResult<Pose>> PoseAsync(this YoloPredictor predictor, Stream stream, YoloConfiguration? configuration = null)
        => PoseAsync(predictor, LoadImage(stream, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<Detection>> DetectAsync(this YoloPredictor predictor, Stream stream, YoloConfiguration? configuration = null)
        => DetectAsync(predictor, LoadImage(stream, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<ObbDetection>> DetectObbAsync(this YoloPredictor predictor, Stream stream, YoloConfiguration? configuration = null)
        => DetectObbAsync(predictor, LoadImage(stream, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<Segmentation>> SegmentAsync(this YoloPredictor predictor, Stream stream, YoloConfiguration? configuration = null)
        => SegmentAsync(predictor, LoadImage(stream, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<Classification>> ClassifyAsync(this YoloPredictor predictor, Stream stream, YoloConfiguration? configuration = null)
        => ClassifyAsync(predictor, LoadImage(stream, configuration ?? predictor.Configuration), configuration);

    #endregion

    #region Predict Image From Buffer

    public static Task<YoloResult<Pose>> PoseAsync(this YoloPredictor predictor, byte[] buffer, YoloConfiguration? configuration = null)
        => PoseAsync(predictor, LoadImage(buffer, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<Detection>> DetectAsync(this YoloPredictor predictor, byte[] buffer, YoloConfiguration? configuration = null)
        => DetectAsync(predictor, LoadImage(buffer, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<ObbDetection>> DetectObbAsync(this YoloPredictor predictor, byte[] buffer, YoloConfiguration? configuration = null)
        => DetectObbAsync(predictor, LoadImage(buffer, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<Segmentation>> SegmentAsync(this YoloPredictor predictor, byte[] buffer, YoloConfiguration? configuration = null)
        => SegmentAsync(predictor, LoadImage(buffer, configuration ?? predictor.Configuration), configuration);

    public static Task<YoloResult<Classification>> ClassifyAsync(this YoloPredictor predictor, byte[] buffer, YoloConfiguration? configuration = null)
        => ClassifyAsync(predictor, LoadImage(buffer, configuration ?? predictor.Configuration), configuration);

    #endregion

    #region Predict Image

    public static Task<YoloResult<Pose>> PoseAsync(this YoloPredictor predictor, Image image, YoloConfiguration? configuration = null)
        => PoseAsync(predictor, image.As<Rgb24>(), configuration);

    public static Task<YoloResult<Detection>> DetectAsync(this YoloPredictor predictor, Image image, YoloConfiguration? configuration = null)
        => DetectAsync(predictor, image.As<Rgb24>(), configuration);

    public static Task<YoloResult<ObbDetection>> DetectObbAsync(this YoloPredictor predictor, Image image, YoloConfiguration? configuration = null)
        => DetectObbAsync(predictor, image.As<Rgb24>(), configuration);

    public static Task<YoloResult<Segmentation>> SegmentAsync(this YoloPredictor predictor, Image image, YoloConfiguration? configuration = null)
        => SegmentAsync(predictor, image.As<Rgb24>(), configuration);

    public static Task<YoloResult<Classification>> ClassifyAsync(this YoloPredictor predictor, Image image, YoloConfiguration? configuration = null)
        => ClassifyAsync(predictor, image.As<Rgb24>(), configuration);

    #endregion

    #region Predict Async Image<Rgb24>

    public static Task<YoloResult<Pose>> PoseAsync(this YoloPredictor predictor, Image<Rgb24> image, YoloConfiguration? configuration = null)
        => Task.Run(() => predictor.Pose(image, configuration));

    public static Task<YoloResult<Detection>> DetectAsync(this YoloPredictor predictor, Image<Rgb24> image, YoloConfiguration? configuration = null)
        => Task.Run(() => predictor.Detect(image, configuration));

    public static Task<YoloResult<ObbDetection>> DetectObbAsync(this YoloPredictor predictor, Image<Rgb24> image, YoloConfiguration? configuration = null)
        => Task.Run(() => predictor.DetectObb(image, configuration));

    public static Task<YoloResult<Segmentation>> SegmentAsync(this YoloPredictor predictor, Image<Rgb24> image, YoloConfiguration? configuration = null)
        => Task.Run(() => predictor.Segment(image, configuration));

    public static Task<YoloResult<Classification>> ClassifyAsync(this YoloPredictor predictor, Image<Rgb24> image, YoloConfiguration? configuration = null)
        => Task.Run(() => predictor.Classify(image, configuration));

    #endregion

    #region LoadImage

    private static Image<Rgb24> LoadImage(string path, YoloConfiguration configuration)
    {
        return configuration.ApplyAutoOrient
               ? Image.Load<Rgb24>(path)
               : Image.Load<Rgb24>(_skipMetadataOptions, path);
    }

    private static Image<Rgb24> LoadImage(Stream stream, YoloConfiguration configuration)
    {
        return configuration.ApplyAutoOrient
               ? Image.Load<Rgb24>(stream)
               : Image.Load<Rgb24>(_skipMetadataOptions, stream);
    }

    private static Image<Rgb24> LoadImage(byte[] buffer, YoloConfiguration configuration)
    {
        return configuration.ApplyAutoOrient
               ? Image.Load<Rgb24>(buffer)
               : Image.Load<Rgb24>(_skipMetadataOptions, buffer);
    }

    #endregion
}