namespace Compunet.YoloSharp.Services;

internal class BoundingBoxTransformer(YoloConfiguration configuration, YoloMetadata metadata) : IBoundingBoxTransformer
{
    public Rectangle Apply(RectangleF rectangle, ImageTransform transform)
    {
        var padding = transform.Padding;
        var ratio = transform.Ratio;

        var x = (rectangle.X - padding.X) * ratio.X;
        var y = (rectangle.Y - padding.Y) * ratio.Y;
        var w = rectangle.Width * ratio.X;
        var h = rectangle.Height * ratio.Y;

        return new Rectangle((int)x, (int)y, (int)w, (int)h);
    }

    public ImageTransform Compute(Size originalImageSize)
    {
        var padding = CalculatePadding(originalImageSize);
        var ratio = CalculateRatio(originalImageSize);

        return new ImageTransform
        {
            Padding = padding,
            Ratio = ratio,
        };
    }

    private Vector<int> CalculatePadding(Size size)
    {
        var model = metadata.ImageSize;

        var xPadding = 0;
        var yPadding = 0;

        if (configuration.KeepAspectRatio)
        {
            var reductionRatio = Math.Min(model.Width / (float)size.Width,
                                          model.Height / (float)size.Height);

            xPadding = (int)((model.Width - size.Width * reductionRatio) / 2);
            yPadding = (int)((model.Height - size.Height * reductionRatio) / 2);
        }

        return (xPadding, yPadding);
    }

    private Vector<float> CalculateRatio(Size size)
    {
        var model = metadata.ImageSize;

        var xRatio = (float)size.Width / model.Width;
        var yRatio = (float)size.Height / model.Height;

        if (configuration.KeepAspectRatio)
        {
            var ratio = Math.Max(xRatio, yRatio);

            xRatio = ratio;
            yRatio = ratio;
        }

        return (xRatio, yRatio);
    }
}
