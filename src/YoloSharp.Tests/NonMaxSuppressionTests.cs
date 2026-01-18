namespace YoloSharp.Tests;

public class NonMaxSuppressionTests
{
    [Fact]
    public void NonMaxSuppressionTest()
    {
        var nonMaxSuppression = new NonMaxSuppressionService();

        var classA = new YoloName(0, "a");
        var classB = new YoloName(1, "b");

        RawBoundingBox[] boxes =
        [
            new RawBoundingBox
            {
                Index = 0,
                NameIndex = classA.Id,
                Bounds = new Rectangle(0, 0, 50, 50),
                Confidence = .8f
            },
            new RawBoundingBox
            {
                Index = 1,
                NameIndex = classA.Id,
                Bounds = new Rectangle(0, 0, 50, 50),
                Confidence = .9f
            },
            new RawBoundingBox
            {
                Index = 2,
                NameIndex = classB.Id,
                Bounds = new Rectangle(0, 0, 50, 50),
                Confidence = .9f
            },
            new RawBoundingBox
            {
                Index = 3,
                NameIndex = classA.Id,
                Bounds = new Rectangle(50, 50, 50, 50),
                Confidence = .5f
            },
        ];

        var selected = nonMaxSuppression.Apply(boxes.AsSpan(), .5f);

        Assert.Equal([1, 2, 3], selected.Select(x => x.Index).Order());
    }
}