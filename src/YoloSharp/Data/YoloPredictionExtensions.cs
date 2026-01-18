namespace Compunet.YoloSharp.Data;

public static class YoloPredictionExtensions
{
    public static Classification GetTopClass(this YoloResult<Classification> result) => result[0];
}