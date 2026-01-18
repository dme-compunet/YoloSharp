namespace Compunet.YoloSharp.Data;

public class YoloResult
{
    public required Size ImageSize { get; init; }

    public required SpeedResult Speed { get; init; }
}