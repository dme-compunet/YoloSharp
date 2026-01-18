namespace YoloSharp.Tests;

public class YoloPredictorTests
{
    [Fact]
    public void PredictorShouldValidateTask()
    {
        const string Path = "./images/bus.jpg";

        Assert.Throws<InvalidOperationException>(() => Predictors.Pose.Detect(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Pose.DetectObb(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Pose.Segment(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Pose.Classify(Path));

        Assert.Throws<InvalidOperationException>(() => Predictors.Detection.Pose(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Detection.DetectObb(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Detection.Segment(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Detection.Classify(Path));

        Assert.Throws<InvalidOperationException>(() => Predictors.ObbDetection.Pose(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.ObbDetection.Detect(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.ObbDetection.Segment(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.ObbDetection.Classify(Path));

        Assert.Throws<InvalidOperationException>(() => Predictors.Segmentation.Pose(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Segmentation.Detect(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Segmentation.DetectObb(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Segmentation.Classify(Path));

        Assert.Throws<InvalidOperationException>(() => Predictors.Classification.Pose(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Classification.Detect(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Classification.DetectObb(Path));
        Assert.Throws<InvalidOperationException>(() => Predictors.Classification.Segment(Path));
    }

    [Theory]
    [InlineData("bus.jpg", 4)]
    [InlineData("sports.jpg", 3)]
    public void Pose(string path, int count)
    {
        path = GetImagePath(path);

        var result = Predictors.Pose.Pose(path);

        Assert.Equal(count, result.Count);
    }

    [Theory]
    [InlineData("bus.jpg", "person:4,bus:1")]
    [InlineData("sports.jpg", "person:2,sports ball:1,baseball bat:1,baseball glove:1")]
    public void Detect(string path, string boxes)
    {
        path = GetImagePath(path);

        var groups = ParseBoxes(boxes).ToArray();

        var result = Predictors.Detection.Detect(path);

        Assert.Equal(groups.Sum(x => x.Count), result.Count);

        foreach (var (name, count) in ParseBoxes(boxes))
        {
            Assert.Equal(count, result.Where(x => x.Name.Name == name).Count());
        }
    }

    [Theory]
    [InlineData("obb.jpg", "ship:84 ,harbor:3,large vehicle:27,small vehicle:54")]
    public void DetectObb(string path, string boxes)
    {
        path = GetImagePath(path);

        var groups = ParseBoxes(boxes).ToArray();

        var result = Predictors.ObbDetection.DetectObb(path);

        Assert.Equal(groups.Sum(x => x.Count), result.Count);

        foreach (var (name, count) in ParseBoxes(boxes))
        {
            Assert.Equal(count, result.Where(x => x.Name.Name == name).Count());
        }
    }

    [Theory]
    [InlineData("bus.jpg", "person:4,bus:1,truck:1")]
    [InlineData("sports.jpg", "person:2,sports ball:1,baseball bat:1,baseball glove:1")]
    public void Segment(string path, string boxes)
    {
        path = GetImagePath(path);

        var groups = ParseBoxes(boxes).ToArray();

        var result = Predictors.Segmentation.Segment(path);

        Assert.Equal(groups.Sum(x => x.Count), result.Count);

        foreach (var (name, count) in ParseBoxes(boxes))
        {
            Assert.Equal(count, result.Where(x => x.Name.Name == name).Count());
        }
    }

    [Theory]
    [InlineData("pizza.jpg", "pizza")]
    [InlineData("teddy.jpg", "teddy")]
    [InlineData("toaster.jpg", "toaster")]
    public void Classify(string path, string label)
    {
        path = GetImagePath(path);

        var result = Predictors.Classification.Classify(path);

        Assert.Equal(result[0].Name.Name, label);
    }

    private static IEnumerable<(string Name, int Count)> ParseBoxes(string boxes)
    {
        foreach (var item in boxes.Split(','))
        {
            var split = item.Split(":");

            var name = split[0];
            var count = int.Parse(split[1]);

            yield return (name, count);
        }
    }

    private static string GetImagePath(string image)
    {
        return Path.Combine("./images", image);
    }
}