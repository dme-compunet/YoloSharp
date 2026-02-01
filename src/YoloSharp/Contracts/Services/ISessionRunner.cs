namespace Compunet.YoloSharp.Contracts.Services;

internal interface ISessionRunner
{
    public IYoloRawOutput PreprocessAndRun(Image<Rgb24> image, out PredictorTimer timer);
}