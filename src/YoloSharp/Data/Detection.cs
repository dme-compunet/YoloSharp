namespace Compunet.YoloSharp.Data;

public class Detection : YoloPrediction, IYoloPrediction<Detection>
{
    public required Rectangle Bounds { get; init; }

    static string IYoloPrediction<Detection>.Describe(Detection[] predictions) => predictions.Summary();
}