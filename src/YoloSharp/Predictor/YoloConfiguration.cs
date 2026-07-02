using SixLabors.ImageSharp.Processing.Processors.Transforms;

namespace Compunet.YoloSharp;

/// <summary>
/// Configuration settings for the YoloPredictor.
/// </summary>
public class YoloConfiguration : IEquatable<YoloConfiguration>
{
    /// <summary>
    /// Default YOLO configuration.
    /// </summary>
    public static readonly YoloConfiguration Default = new();

    /// <summary>
    /// Specify the minimum confidence value for including a result. Default is 0.3f.
    /// </summary>
    public float Confidence { get; set; } = .3f;

    /// <summary>
    /// Specify the minimum IoU value for Non-Maximum Suppression (NMS). Default is 0.45f.
    /// </summary>
    public float IoU { get; set; } = .45f;

    /// <summary>
    /// Specify whether to keep the image aspect ratio when resizing. Default is true.
    /// </summary>
    public bool KeepAspectRatio { get; set; } = true;

    /// <summary>
    /// Specify whether to apply automatic image orientation correction on load. Default is true.
    /// </summary>
    public bool ApplyAutoOrient { get; set; } = true;

    /// <summary>
    /// Specify whether to suppress parallel inference (pre-processing and post-processing will run in parallelly). Default is false.
    /// </summary>
    public bool SuppressParallelInference { get; set; } = false;

    /// <summary>
    /// The resampling algorithm. Bicubic - recommended in the ImageSharp documentation
    /// </summary>
    public YoloImageResampler ImageResampler { get; set; } = YoloImageResampler.NearestNeighbor;

    internal IResampler GetImageResampler() => ImageResampler switch
    {
        YoloImageResampler.Bicubic => KnownResamplers.Bicubic,
        YoloImageResampler.NearestNeighbor => KnownResamplers.NearestNeighbor,
        YoloImageResampler.Box => KnownResamplers.Box,
        YoloImageResampler.Triangle => KnownResamplers.Triangle,
        YoloImageResampler.Hermite => KnownResamplers.Hermite,
        YoloImageResampler.Lancos2 => KnownResamplers.Lanczos2,
        YoloImageResampler.Lanczos3 => KnownResamplers.Lanczos3,
        YoloImageResampler.Lanczos5 => KnownResamplers.Lanczos5,
        YoloImageResampler.Lanczos8 => KnownResamplers.Lanczos8,
        YoloImageResampler.Spline => KnownResamplers.Spline,
        _ => KnownResamplers.NearestNeighbor
    };

    public bool Equals(YoloConfiguration? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return Confidence == other.Confidence
               && IoU == other.IoU
               && KeepAspectRatio == other.KeepAspectRatio
               && ApplyAutoOrient == other.ApplyAutoOrient
               && SuppressParallelInference == other.SuppressParallelInference;
    }

    public override bool Equals(object? obj) => Equals(obj as YoloConfiguration);

    public override int GetHashCode()
    {
        return Confidence.GetHashCode()
               ^ IoU.GetHashCode()
               ^ KeepAspectRatio.GetHashCode()
               ^ ApplyAutoOrient.GetHashCode()
               ^ SuppressParallelInference.GetHashCode();
    }
}
