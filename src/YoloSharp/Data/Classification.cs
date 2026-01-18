
namespace Compunet.YoloSharp.Data;

public class Classification : YoloPrediction, IYoloPrediction<Classification>
{
    static string IYoloPrediction<Classification>.Describe(Classification[] predictions) => predictions[0].ToString();
}