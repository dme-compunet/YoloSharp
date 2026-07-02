namespace Compunet.YoloSharp.Metadata;

/// <summary>
/// Specifies the resampling algorithm used when resizing input images.
/// </summary>
public enum YoloImageResampler
{
    Bicubic,
    Triangle,
    NearestNeighbor,
    Lanczos2,
    Lanczos3,
    Lanczos5,
    Lanczos8,
    Spline,
    Box,
    Hermite
}
