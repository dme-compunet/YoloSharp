namespace Compunet.YoloSharp.Services.Plotting;

internal class MaskDrawer(IImageContoursRecognizer contoursRecognizer) : IMaskDrawer
{
    public void DrawMask(Segmentation prediction, PlottingContext context)
    {
        var mask = prediction.Mask;
        var size = context.Target.Size;

        var color = context.ColorPalette.GetColor(prediction.Name.Id);

        using var maskLayer = new Image<Rgba32>(prediction.Bounds.Width, prediction.Bounds.Height);

        for (var x = 0; x < mask.Width; x++)
        {
            for (var y = 0; y < mask.Height; y++)
            {
                var value = mask[y, x];

                if (value > context.MaskMinimumConfidence)
                {
                    maskLayer[x, y] = color;
                }
            }
        }

        if (context.MaskAlpha > 0)
        {
            context.Target.Mutate(x => x.DrawImage(maskLayer, prediction.Bounds.Location, context.MaskAlpha));
        }

        if (context.ContoursThickness > 0)
        {
            var contours = contoursRecognizer.Recognize(maskLayer);

            using var contoursLayer = new Image<Rgba32>(maskLayer.Width, maskLayer.Height);

            foreach (var points in contours)
            {
                if (points.Length < 2)
                {
                    continue;
                }

                var path = new PathBuilder().AddLines(points.Select(point => (PointF)point)).Build();

                contoursLayer.Mutate(x =>
                {
                    x.Draw(color, context.ContoursThickness, path);
                });
            }

            context.Target.Mutate(x => x.DrawImage(contoursLayer, prediction.Bounds.Location, 1));
        }
    }
}