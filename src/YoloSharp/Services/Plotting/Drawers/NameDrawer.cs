namespace Compunet.YoloSharp.Services.Plotting;

internal class NameDrawer : INameDrawer
{
    private static readonly Color ForegroundColor = Color.White;

    public void DrawName(YoloPrediction prediction, PointF position, bool inside, PlottingContext context)
    {
        var font = context.TextOptions.Font;
        
        if (font.Size <= 0)
        {
            return;
        }
        
        var xPadding = context.NamePadding.X;
        var yPadding = context.NamePadding.Y;

        var text = prediction.ToString();

        // Measure the text bounds
        var bounds = TextMeasurer.MeasureBounds(text, context.TextOptions);
        var size = new SizeF(bounds.Width + xPadding, font.Size + yPadding);

        // Move the position up
        if (inside == false)
        {
            position.Offset(0, -size.Height);
        }

        var textPosition = new PointF(position.X + xPadding / 2, position.Y + yPadding / 2);
        var textBoxPolygon = new RectangularPolygon(position, size);

        // Fix text position
        textPosition.Offset(0, -(yPadding * .1f));

        var foreground = ForegroundColor;
        var background = context.ColorPalette.GetColor(prediction.Name.Id);

        context.Target.Mutate(x =>
        {
            x.Fill(background.WithAlpha(.5f), textBoxPolygon);

            if (context.BorderThickness > 0)
            {
                x.Draw(background, context.BorderThickness, textBoxPolygon);
            }

            // Draw the text
            x.DrawText(text, font, foreground, textPosition);
        });
    }
}