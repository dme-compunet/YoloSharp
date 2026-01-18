namespace Compunet.YoloSharp.Metadata;

public class YoloPoseMetadata : YoloMetadata
{
    public KeypointShape KeypointShape { get; }

    internal YoloPoseMetadata(InferenceSession session) : base(session)
    {
        var metadata = session.ModelMetadata.CustomMetadataMap;

        KeypointShape = ParseKeypointShape(metadata["kpt_shape"]);
    }

    private static KeypointShape ParseKeypointShape(string text)
    {
        text = text[1..^1]; // '[17, 3]' => '17, 3'

        var split = text.Split(", ");

        var count = int.Parse(split[0]);
        var channels = int.Parse(split[1]);

        return new KeypointShape(count, channels);
    }
}