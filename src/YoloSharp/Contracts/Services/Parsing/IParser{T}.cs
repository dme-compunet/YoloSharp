namespace Compunet.YoloSharp.Contracts.Services;

internal interface IParser<T> where T : IYoloPrediction<T>
{
    public T[] ProcessTensorToResult(IYoloRawOutput output, Size size);
}