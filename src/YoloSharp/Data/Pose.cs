namespace Compunet.YoloSharp.Data;

public class Pose(Keypoint[] keypoints) : Detection, IYoloPrediction<Pose>, IEnumerable<Keypoint>
{
    public Keypoint this[int index] => keypoints[index];

    static string IYoloPrediction<Pose>.Describe(Pose[] predictions) => predictions.Summary();

    #region Enumerator

    public IEnumerator<Keypoint> GetEnumerator()
    {
        foreach (var item in keypoints)
        {
            yield return item;
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    #endregion
}