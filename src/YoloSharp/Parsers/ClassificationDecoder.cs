namespace Compunet.YoloSharp.Parsers;

internal class ClassificationDecoder(YoloMetadata metadata) : IDecoder<Classification>
{
    public Classification[] Decode(IYoloRawOutput tensor, Size size)
    {
        var tensorSpan = tensor.Output0.Span;

        var result = new Classification[tensorSpan.Length];

        for (var i = 0; i < tensorSpan.Length; i++)
        {
            var name = metadata.Names[i];
            var confidence = tensorSpan[i];

            result[i] = new Classification
            {
                Name = name,
                Confidence = confidence,
            };
        }

        result.AsSpan().Sort((x, y) => y.Confidence.CompareTo(x.Confidence));

        return result;
    }
}