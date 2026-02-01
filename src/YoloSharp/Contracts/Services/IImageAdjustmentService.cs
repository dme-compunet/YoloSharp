namespace Compunet.YoloSharp.Contracts.Services;

internal interface IBoundingBoxTransformer
{
    public Rectangle Apply(RectangleF rectangle, ImageTransform transform);

    public ImageTransform Compute(Size originalImageSize);
}