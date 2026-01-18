namespace Compunet.YoloSharp.Services.Plotting;

internal class BoxDrawer : IBoxDrawer
{
    private const float BoxFillAlpha = .1f;

    public void DrawBox(Detection prediction, PlottingContext context)
    {
        var points = GetBoxPoints(prediction);

        DrawBox(prediction, points, context);
    }

    public void DrawBox(Detection prediction, PointF[] points, PlottingContext context)
    {
        if (context.BorderThickness <= 0)
        {
            return;
        }
        
        var polygon = new Polygon(points);
        var color = context.ColorPalette.GetColor(prediction.Name.Id);

        context.Target.Mutate(x =>
        {
            // Fill the box
            x.Fill(color.WithAlpha(BoxFillAlpha), polygon);

            // Draw the box bounds
            x.Draw(color, context.BorderThickness, polygon);
        });
    }

    private static PointF[] GetBoxPoints(Detection detection)
    {
        var rectangle = detection.Bounds;

        return
        [
            new PointF(rectangle.Left, rectangle.Top),
            new PointF(rectangle.Right, rectangle.Top),
            new PointF(rectangle.Right, rectangle.Bottom),
            new PointF(rectangle.Left, rectangle.Bottom),
        ];
    }
}