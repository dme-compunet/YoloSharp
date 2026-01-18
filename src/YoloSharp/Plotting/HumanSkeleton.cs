namespace Compunet.YoloSharp.Plotting;

internal class HumanSkeleton : ISkeleton
{
    private readonly string[] _colors =
    [
        "FF8000",
        "FF9933",
        "FFB266",
        "E6E600",
        "FF99FF",
        "99CCFF",
        "FF66FF",
        "FF33FF",
        "66B2FF",
        "3399FF",
        "FF9999",
        "FF6666",
        "FF3333",
        "99FF99",
        "66FF66",
        "33FF33",
        "00FF00",
        "0000FF",
        "FF0000",
        "FFFFFF",
    ];

    private readonly SkeletonConnection[] _connections =
    [
        (15, 13),
        (13, 11),
        (16, 14),
        (14, 12),
        (11, 12),
        (5, 11),
        (6, 12),
        (5, 6),
        (5, 7),
        (6, 8),
        (7, 9),
        (8, 10),
        (1, 2),
        (0, 1),
        (0, 2),
        (1, 3),
        (2, 4),
        (3, 5),
        (4, 6)
    ];

    private readonly int[] _keypointColorMap =
    [
        16, 16, 16, 16, 16, 0, 0, 0, 0, 0, 0, 9, 9, 9, 9, 9, 9
    ];

    private readonly int[] _lineColorMap =
    [
        9, 9, 9, 9, 7, 7, 7, 0, 0, 0, 0, 0, 16, 16, 16, 16, 16, 16, 16
    ];

    public SkeletonConnection[] Connections => _connections;

    public Color GetKeypointColor(int index)
    {
        index = _keypointColorMap[index % _keypointColorMap.Length];

        var hex = _colors[index];

        return Color.ParseHex(hex);
    }

    public Color GetLineColor(int index)
    {
        index = _lineColorMap[index % _lineColorMap.Length];

        var hex = _colors[index];

        return Color.ParseHex(hex);
    }
}