namespace Compunet.YoloSharp.Metadata;

public class YoloMetadata
{
    public string Author { get; }

    public string Description { get; }

    public string Version { get; }

    public int BatchSize { get; }

    public Size ImageSize { get; }

    public YoloTask Task { get; }

    public YoloName[] Names { get; }

    public YoloArchitecture Architecture { get; }

    internal bool IsEndToEnd { get; }

    internal int BatchAxis { get; }

    internal int PredictionAxis { get; }

    internal int FeatureAxis { get; }

    internal int AttributeOffset { get; }

    internal YoloMetadata(InferenceSession session)
        :
        this(session, ParseYoloArchitecture(session))
    { }

    internal YoloMetadata(InferenceSession session, YoloArchitecture architecture)
    {
        var metadata = session.ModelMetadata.CustomMetadataMap;

        Author = metadata["author"];
        Description = metadata["description"];
        Version = metadata["version"];

        Task = metadata["task"] switch
        {
            "obb" => YoloTask.Obb,
            "pose" => YoloTask.Pose,
            "detect" => YoloTask.Detect,
            "segment" => YoloTask.Segment,
            "classify" => YoloTask.Classify,
            _ => throw new InvalidOperationException("Unknow YOLO 'task' value")
        };

        //Architecture = architecture;
        BatchSize = int.Parse(metadata["batch"]);
        ImageSize = ParseSize(metadata["imgsz"]);
        Names = ParseNames(metadata["names"]);

        if (metadata.TryGetValue("end2end", out var value))
        {
            IsEndToEnd = value.Equals("true", StringComparison.CurrentCultureIgnoreCase);
        }

        Architecture = DetectArchitecture(Names.Length, session.OutputMetadata["output0"].Dimensions, Task);

        BatchAxis = 0;

        if (Architecture == YoloArchitecture.AnchorFree)
        {
            PredictionAxis = 1;
            FeatureAxis = 2;
            AttributeOffset = 6;
        }
        else
        {
            PredictionAxis = 2;
            FeatureAxis = 1;
            AttributeOffset = 4 + Names.Length;
        }
    }

    public static YoloMetadata Parse(InferenceSession session)
    {
        try
        {
            if (session.ModelMetadata.CustomMetadataMap["task"] == "pose")
            {
                return new YoloPoseMetadata(session);
            }

            return new YoloMetadata(session);
        }
        catch (Exception inner)
        {
            throw new InvalidOperationException("The metadata parsing failed, making sure you use an official YOLO model", inner);
        }
    }

    private static YoloArchitecture DetectArchitecture(int outputCount, int[] shape, YoloTask task)
    {
        // TODO

        if (shape.Length != 3)
        {
            return YoloArchitecture.Unknown;
        }

        if (shape[1] == 300)
        {
            return YoloArchitecture.AnchorFree;
        }

        return YoloArchitecture.AnchorBased;
    }

    private static YoloArchitecture ParseYoloArchitecture(InferenceSession session)
    {
        var metadata = session.ModelMetadata.CustomMetadataMap;

        if (metadata.TryGetValue("task", out var task) == false)
        {
            throw new InvalidOperationException();
        }

        if (task == "detect")
        {
            var output0 = session.OutputMetadata["output0"];

            if (output0.Dimensions[2] == 6) // YOLOv10 output0: [1, 300, 6]
            {
                return YoloArchitecture.YoloV10;
            }
        }

        return YoloArchitecture.Ultralytics;
    }

    #region Parsers

    private static Size ParseSize(string text)
    {
        text = text[1..^1]; // '[640, 640]' => '640, 640'

        var split = text.Split(", ");

        var y = int.Parse(split[0]);
        var x = int.Parse(split[1]);

        return new Size(x, y);
    }

    private static YoloName[] ParseNames(string text)
    {
        text = text[1..^1];

        var split = text.Split(", ");
        var count = split.Length;

        var names = new YoloName[count];

        for (int i = 0; i < count; i++)
        {
            var value = split[i];

            var valueSplit = value.Split(": ");

            var id = int.Parse(valueSplit[0]);
            var name = valueSplit[1][1..^1].Replace('_', ' ');

            names[id] = new YoloName(id, name);
        }

        return names;
    }

    #endregion
}