namespace Compunet.YoloSharp;

internal readonly struct ImageTransform
{
    public Vector<int> Padding { get; init; }

    public Vector<float> Ratio { get; init; }
}