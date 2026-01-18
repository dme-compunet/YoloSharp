namespace Compunet.YoloSharp.Contracts.Services.Plotting;

internal interface IImageContoursRecognizer
{
    public Point[][] Recognize(Image luminance);
}