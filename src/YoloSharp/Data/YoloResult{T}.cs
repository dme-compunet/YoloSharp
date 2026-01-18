namespace Compunet.YoloSharp.Data;

public class YoloResult<TPrediction>(TPrediction[] predictions) : YoloResult, IEnumerable<TPrediction> where TPrediction : IYoloPrediction<TPrediction>
{
    public TPrediction this[int index] => predictions[index];

    public int Count => predictions.Length;

    public override string ToString() => TPrediction.Describe(predictions);

    #region Enumerator

    public IEnumerator<TPrediction> GetEnumerator()
    {
        foreach (var item in predictions)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}