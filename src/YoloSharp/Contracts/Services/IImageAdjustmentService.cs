namespace Compunet.YoloSharp.Contracts.Services;

internal interface IImageAdjustmentService
{
    public ImageAdjustmentInfo Calculate(Size size);

    public Rectangle Adjust(RectangleF rectangle, ImageAdjustmentInfo adjustment);
}