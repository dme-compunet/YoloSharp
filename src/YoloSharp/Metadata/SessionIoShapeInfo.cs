namespace Compunet.YoloSharp.Metadata;

internal class SessionIoShapeInfo
{
    public TensorShape Input0 { get; }

    public TensorShape Output0 { get; }

    public TensorShape? Output1 { get; }

    public bool IsDynamicOutput
    {
        get
        {
            if (Output0.IsDynamic)
            {
                return true;
            }

            if (Output1 != null && Output1.Value.IsDynamic)
            {
                return true;
            }

            return false;
        }
    }

    public SessionIoShapeInfo(InferenceSession session, YoloMetadata metadata)
    {
        var inputMetadata = session.InputMetadata.Values;
        var outputMetadata = session.OutputMetadata.Values;

        var input0 = new TensorShape(inputMetadata.First().Dimensions);

        if (input0.IsDynamic)
        {
            Input0 = new TensorShape([metadata.BatchSize, 3, metadata.ImageSize.Height, metadata.ImageSize.Width]);
        }
        else
        {
            Input0 = input0;
        }

        Output0 = new TensorShape(outputMetadata.First().Dimensions);

        if (session.OutputMetadata.Count == 2)
        {
            Output1 = new TensorShape(outputMetadata.Last().Dimensions);
        }
    }
}