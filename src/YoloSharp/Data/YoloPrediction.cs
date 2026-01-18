namespace Compunet.YoloSharp.Data;

public abstract class YoloPrediction
{
    public required YoloName Name { get; init; }

    public required float Confidence { get; init; }

    public override string ToString()
    {
        return $"{Name.Name} ({(int)(Confidence * 100f)}%)";
    }
}