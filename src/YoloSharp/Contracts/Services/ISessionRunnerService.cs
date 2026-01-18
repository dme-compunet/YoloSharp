namespace Compunet.YoloSharp.Contracts.Services;

internal interface ISessionRunnerService
{
    public IYoloRawOutput PreprocessAndRun(Image<Rgb24> image, out PredictorTimer timer);
}