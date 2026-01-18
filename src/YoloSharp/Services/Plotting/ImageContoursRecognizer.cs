namespace Compunet.YoloSharp.Services.Plotting;

internal class ImageContoursRecognizer : IImageContoursRecognizer
{
    private readonly (Func<Point, Point> Func, int Neighborhood)[] _neighborhood;

    public ImageContoursRecognizer()
    {
        _neighborhood =
        [
            (point => new Point(point.X - 1, point.Y), 7),
            (point => new Point(point.X - 1, point.Y - 1), 7),
            (point => new Point(point.X, point.Y - 1), 1),
            (point => new Point(point.X + 1, point.Y - 1), 1),
            (point => new Point(point.X + 1, point.Y), 3),
            (point => new Point(point.X + 1, point.Y + 1), 3),
            (point => new Point(point.X, point.Y+1), 5),
            (point => new Point(point.X -1, point.Y + 1), 5)
        ];
    }

    public Point[][] Recognize(Image image)
    {
        using var luminance = image.CloneAs<L8>();

        var found = new HashSet<Point>();
        var contours = new List<Point[]>();
        var inside = false;

        for (var y = 0; y < luminance.Height; y++)
        {
            for (var x = 0; x < luminance.Width; x++)
            {
                var point = new Point(x, y);

                if (found.Contains(point) && !inside)
                {
                    inside = true;
                    continue;
                }

                var transparent = IsTransparent(luminance, point);

                if (!transparent && inside)
                {
                    continue;
                }

                if (transparent && inside)
                {
                    inside = false;
                    continue;
                }

                if (!transparent && !inside)
                {
                    var contour = new List<Point>();

                    found.Add(point);
                    contour.Add(point);

                    var checkLocationNr = 1;
                    var startPos = point;

                    var counter1 = 0;
                    var counter2 = 0;

                    while (true)
                    {
                        var checkPosition = _neighborhood[checkLocationNr - 1].Func(point);
                        var newCheckLocationNr = _neighborhood[checkLocationNr - 1].Neighborhood;

                        if (!IsTransparent(luminance, checkPosition))
                        {
                            if (checkPosition == startPos)
                            {
                                counter1++;

                                if (newCheckLocationNr == 1 || counter1 >= 3)
                                {
                                    inside = true;
                                    break;
                                }
                            }

                            checkLocationNr = newCheckLocationNr;
                            point = checkPosition;
                            counter2 = 0;
                            found.Add(point);
                            contour.Add(point);
                        }
                        else
                        {
                            checkLocationNr = 1 + (checkLocationNr % 8);

                            if (counter2 > 8)
                            {
                                break;
                            }
                            else
                            {
                                counter2++;
                            }
                        }
                    }

                    contours.Add([.. contour]);
                }
            }
        }

        return [.. contours];
    }

    private static bool IsTransparent(Image<L8> image, Point pixel)
    {
        return pixel.X > image.Width - 1
               || pixel.X < 0
               || pixel.Y > image.Height - 1
               || pixel.Y < 0
               || image[pixel.X, pixel.Y].PackedValue == 0;
    }
}