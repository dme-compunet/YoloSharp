namespace Compunet.YoloSharp.Metadata;

public enum YoloArchitecture
{
    Unknown,
    /// <summary>
    /// YOLOv10
    /// YOLO26
    /// </summary>
    AnchorFree,

    /// <summary>
    /// YOLOv8
    /// YOLO11
    /// YOLOv12
    /// </summary>
    AnchorBased,

    /// <summary>
    /// YOLOv8, YOLO11, YOLOv12
    /// </summary>
    Ultralytics,
    YoloV10
}