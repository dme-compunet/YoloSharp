namespace Compunet.YoloSharp.Contracts.Services;

internal interface IDecoder<T> where T : IYoloPrediction<T>
{
    public T[] Decode(IYoloRawOutput output, Size size);
}