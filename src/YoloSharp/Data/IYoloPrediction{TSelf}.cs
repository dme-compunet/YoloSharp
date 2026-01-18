namespace Compunet.YoloSharp.Data;

public interface IYoloPrediction<TSelf>
{
    internal abstract static string Describe(TSelf[] predictions);
}