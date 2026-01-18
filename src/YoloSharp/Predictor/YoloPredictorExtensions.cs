namespace Compunet.YoloSharp;

public static class YoloPredictorExtensions
{
    private static readonly DecoderOptions _skipMetadataOptions = new()
    {
        SkipMetadata = true,
    };

    #region Predict Image From Path

    public static YoloResult<Pose> Pose(this YoloPredictor predictor, string path, YoloConfiguration? configuration = null)
        => Pose(predictor, LoadImage(path, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<Detection> Detect(this YoloPredictor predictor, string path, YoloConfiguration? configuration = null)
        => Detect(predictor, LoadImage(path, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<ObbDetection> DetectObb(this YoloPredictor predictor, string path, YoloConfiguration? configuration = null)
        => DetectObb(predictor, LoadImage(path, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<Segmentation> Segment(this YoloPredictor predictor, string path, YoloConfiguration? configuration = null)
        => Segment(predictor, LoadImage(path, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<Classification> Classify(this YoloPredictor predictor, string path, YoloConfiguration? configuration = null)
        => Classify(predictor, LoadImage(path, configuration ?? predictor.Configuration), configuration);

    #endregion

    #region Predict Image From Stream

    public static YoloResult<Pose> Pose(this YoloPredictor predictor, Stream stream, YoloConfiguration? configuration = null)
        => Pose(predictor, LoadImage(stream, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<Detection> Detect(this YoloPredictor predictor, Stream stream, YoloConfiguration? configuration = null)
        => Detect(predictor, LoadImage(stream, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<ObbDetection> DetectObb(this YoloPredictor predictor, Stream stream, YoloConfiguration? configuration = null)
        => DetectObb(predictor, LoadImage(stream, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<Segmentation> Segment(this YoloPredictor predictor, Stream stream, YoloConfiguration? configuration = null)
        => Segment(predictor, LoadImage(stream, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<Classification> Classify(this YoloPredictor predictor, Stream stream, YoloConfiguration? configuration = null)
        => Classify(predictor, LoadImage(stream, configuration ?? predictor.Configuration), configuration);

    #endregion

    #region Predict Image From Buffer

    public static YoloResult<Pose> Pose(this YoloPredictor predictor, byte[] buffer, YoloConfiguration? configuration = null)
        => Pose(predictor, LoadImage(buffer, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<Detection> Detect(this YoloPredictor predictor, byte[] buffer, YoloConfiguration? configuration = null)
        => Detect(predictor, LoadImage(buffer, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<ObbDetection> DetectObb(this YoloPredictor predictor, byte[] buffer, YoloConfiguration? configuration = null)
        => DetectObb(predictor, LoadImage(buffer, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<Segmentation> Segment(this YoloPredictor predictor, byte[] buffer, YoloConfiguration? configuration = null)
        => Segment(predictor, LoadImage(buffer, configuration ?? predictor.Configuration), configuration);

    public static YoloResult<Classification> Classify(this YoloPredictor predictor, byte[] buffer, YoloConfiguration? configuration = null)
        => Classify(predictor, LoadImage(buffer, configuration ?? predictor.Configuration), configuration);

    #endregion

    #region Predict Image

    public static YoloResult<Pose> Pose(this YoloPredictor predictor, Image image, YoloConfiguration? configuration = null) => Pose(predictor, image.As<Rgb24>(), configuration);

    public static YoloResult<Detection> Detect(this YoloPredictor predictor, Image image, YoloConfiguration? configuration = null) => Detect(predictor, image.As<Rgb24>(), configuration);

    public static YoloResult<ObbDetection> DetectObb(this YoloPredictor predictor, Image image, YoloConfiguration? configuration = null) => DetectObb(predictor, image.As<Rgb24>(), configuration);

    public static YoloResult<Segmentation> Segment(this YoloPredictor predictor, Image image, YoloConfiguration? configuration = null) => Segment(predictor, image.As<Rgb24>(), configuration);

    public static YoloResult<Classification> Classify(this YoloPredictor predictor, Image image, YoloConfiguration? configuration = null) => Classify(predictor, image.As<Rgb24>(), configuration);

    #endregion

    #region Predict Image<Rgb24>

    public static YoloResult<Pose> Pose(this YoloPredictor predictor, Image<Rgb24> image, YoloConfiguration? configuration = null)
        => predictor.Predict<Pose>(image, configuration);

    public static YoloResult<Detection> Detect(this YoloPredictor predictor, Image<Rgb24> image, YoloConfiguration? configuration = null)
        => predictor.Predict<Detection>(image, configuration);

    public static YoloResult<ObbDetection> DetectObb(this YoloPredictor predictor, Image<Rgb24> image, YoloConfiguration? configuration = null)
        => predictor.Predict<ObbDetection>(image, configuration);

    public static YoloResult<Segmentation> Segment(this YoloPredictor predictor, Image<Rgb24> image, YoloConfiguration? configuration = null)
        => predictor.Predict<Segmentation>(image, configuration);

    public static YoloResult<Classification> Classify(this YoloPredictor predictor, Image<Rgb24> image, YoloConfiguration? configuration = null)
        => predictor.Predict<Classification>(image, configuration);

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