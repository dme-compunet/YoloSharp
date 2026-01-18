namespace Compunet.YoloSharp.Extensions;

internal static class ImageSharpExtensions
{
    public static Image<TPixel> As<TPixel>(this Image image) where TPixel : unmanaged, IPixel<TPixel>
    {
        if (image is Image<TPixel> result)
        {
            return result;
        }

        return image.CloneAs<TPixel>();
    }

    public static void AutoOrient(this Image image) => image.Mutate(x => x.AutoOrient());
}