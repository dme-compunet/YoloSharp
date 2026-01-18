namespace Compunet.YoloSharp;

internal ref struct PredictorTimer()
{
    private readonly Stopwatch _stopwatch = new();

    private TimeSpan _preprocess;
    private TimeSpan _inference;
    private TimeSpan _postprocess;

    public readonly void StartPreprocess()
    {
        _stopwatch.Restart();
    }

    public void StartInference()
    {
        _preprocess = _stopwatch.Elapsed;
        _stopwatch.Restart();
    }

    public void StartPostprocess()
    {
        _inference = _stopwatch.Elapsed;
        _stopwatch.Restart();
    }

    public SpeedResult Stop()
    {
        _postprocess = _stopwatch.Elapsed;
        _stopwatch.Stop();

        return new SpeedResult(_preprocess, _inference, _postprocess);
    }
}