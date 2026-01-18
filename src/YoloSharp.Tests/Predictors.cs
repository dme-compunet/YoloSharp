namespace YoloSharp.Tests;

public static class Predictors
{
    public static YoloPredictor Pose { get; } = new("./models/yolo11n-pose.onnx");
    public static YoloPredictor Detection { get; } = new("./models/yolo11n.onnx");
    public static YoloPredictor ObbDetection { get; } = new("./models/yolo11n-obb.onnx");
    public static YoloPredictor Segmentation { get; } = new("./models/yolo11n-seg.onnx");
    public static YoloPredictor Classification { get; } = new("./models/yolo11n-cls.onnx");

    public static YoloPredictor GetPredictor(YoloTask task)
    {
        return task switch
        {
            YoloTask.Pose => Pose,
            YoloTask.Detect => Detection,
            YoloTask.Obb => ObbDetection,
            YoloTask.Segment => Segmentation,
            YoloTask.Classify => Classification,
            _ => throw new InvalidEnumArgumentException()
        };
    }
}