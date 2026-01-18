namespace Compunet.YoloSharp.Extensions;

internal static class DetectionBoxesExtensions
{
    public static string Summary(this IEnumerable<Detection> boxes)
    {
        var sort = boxes.Select(x => x.Name)
                        .GroupBy(x => x.Id)
                        .OrderBy(x => x.Key)
                        .Select(x => $"{x.Count()} {x.First().Name}");

        return string.Join(", ", sort);
    }
}