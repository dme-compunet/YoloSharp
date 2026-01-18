namespace Compunet.YoloSharp;

internal readonly struct ImageAdjustmentInfo(Vector<int> padding, Vector<float> ratio)
{
    public Vector<int> Padding { get; } = padding;

    public Vector<float> Ratio { get; } = ratio;
}