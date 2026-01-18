using Compunet.YoloSharp;
using Compunet.YoloSharp.Plotting;
using System.Diagnostics;

Console.WriteLine("Loading pose estimation model...");
using var posePredictor = new YoloPredictor("./models/yolo11n-pose.onnx");

Console.WriteLine("Loading detection model...");
using var detectPredictor = new YoloPredictor("./models/yolo11n.onnx");

Console.WriteLine("Loading obb detection model...");
using var obbPredictor = new YoloPredictor("./models/yolo11n-obb.onnx");

Console.WriteLine("Loading segmentation model...");
using var segmentPredictor = new YoloPredictor("./models/yolo11n-seg.onnx");

Console.WriteLine("Loading classification model...");
using var classifyPredictor = new YoloPredictor("./models/yolo11n-cls.onnx");

Console.WriteLine();

await PredictAndSaveAsync(posePredictor, "bus.jpg");
await PredictAndSaveAsync(posePredictor, "sports.jpg");

await PredictAndSaveAsync(detectPredictor, "bus.jpg");
await PredictAndSaveAsync(detectPredictor, "sports.jpg");

await PredictAndSaveAsync(obbPredictor, "obb.jpg");

await PredictAndSaveAsync(segmentPredictor, "sports.jpg");

await PredictAndSaveAsync(classifyPredictor, "pizza.jpg");
await PredictAndSaveAsync(classifyPredictor, "teddy.jpg");
await PredictAndSaveAsync(classifyPredictor, "toaster.jpg");

if (OperatingSystem.IsWindows())
{
    Process.Start(new ProcessStartInfo
    {
        FileName = Path.GetFullPath("./images"),
        UseShellExecute = true,
    });
}

static async Task PredictAndSaveAsync(YoloPredictor predictor, string image)
{
    var path = $"./images/{image}";
    var task = predictor.Metadata.Task;

    Console.WriteLine($"Running '{image}' (test: {task})...");

    var result = await predictor.PredictAndSaveAsync(path);

    Console.WriteLine($"Result: {result}");
    Console.WriteLine($"Speed:  {result.Speed}");

    Console.WriteLine();
}