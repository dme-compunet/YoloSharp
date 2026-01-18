namespace Compunet.YoloSharp.Data;

public class Keypoint
{
    public required int Index { get; init; }

    public required Point Point { get; init; }

    public required float Confidence { get; init; }
}