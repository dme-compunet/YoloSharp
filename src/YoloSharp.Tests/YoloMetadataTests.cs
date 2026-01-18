namespace YoloSharp.Tests;

public class YoloMetadataTests
{
    [Fact]
    public void PredictorTaskTest()
    {
        Assert.Equal(YoloTask.Pose, Predictors.Pose.Metadata.Task);
        Assert.Equal(YoloTask.Detect, Predictors.Detection.Metadata.Task);
        Assert.Equal(YoloTask.Obb, Predictors.ObbDetection.Metadata.Task);
        Assert.Equal(YoloTask.Segment, Predictors.Segmentation.Metadata.Task);
        Assert.Equal(YoloTask.Classify, Predictors.Classification.Metadata.Task);
    }

    [Fact]
    public void MetadataParsingTest()
    {
        var author = "Ultralytics";
        var description = "Ultralytics YOLOv8n model trained on coco.yaml";
        var version = "8.2.79";
        var batch = 1;
        var imgsz = new Size(640, 1024);
        var task = YoloTask.Detect;
        var names = new YoloName[]
        {
            new(0, "person"),
            new(1, "bicycle"),
        };

        var dictionary = new Dictionary<string, string>()
        {
            { nameof(author), author },
            { nameof(description), description },
            { nameof(version), version },
            { nameof(batch), $"{batch}" },
            { nameof(imgsz), $"[{imgsz.Height}, {imgsz.Width}]" },
            { nameof(task), $"{task.ToString().ToLower()}" },
            { nameof(names), $"{{{names[0]}, {names[1]}}}" }
        };

        var metadata = new YoloMetadata(dictionary, YoloArchitecture.Ultralytics);

        Assert.Equal(author, metadata.Author);
        Assert.Equal(description, metadata.Description);
        Assert.Equal(version, metadata.Version);
        Assert.Equal(batch, metadata.BatchSize);
        Assert.Equal(imgsz, metadata.ImageSize);
        Assert.Equal(task, metadata.Task);

        Assert.Equal(names.Length, metadata.Names.Length);
        Assert.Equal($"{names[0]}", $"{metadata.Names[0]}");
        Assert.Equal($"{names[1]}", $"{metadata.Names[1]}");

        Assert.Equal(YoloArchitecture.Ultralytics, metadata.Architecture);
    }
}