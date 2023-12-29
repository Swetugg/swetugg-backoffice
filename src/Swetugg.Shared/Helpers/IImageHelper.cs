using System.Threading.Tasks;

namespace Swetugg.Shared.Helpers;

/// <summary>
/// Image manipulation operations.
/// </summary>
public interface IImageHelper
{
    /// <summary>
    /// Resizes an image.
    /// </summary>
    /// <param name="image">The image to resize.</param>
    /// <param name="width">The desired width of the image.</param>
    /// <param name="height">The desired height of the image.</param>
    /// <param name="pad">Specifies whether to pad the image to fit the desired dimensions.</param>
    /// <returns>The resized image.</returns>
    Task<byte[]> ResizeImage(byte[] image, int? width, int? height, bool pad);

    /// <summary>
    /// Sets the format of an image.
    /// </summary>
    /// <param name="image">The image to set the format for.</param>
    /// <param name="format">The desired image format.</param>
    /// <returns>The image with the specified format.</returns>
    Task<byte[]> SetFormat(byte[] image, ImageFormat format);
}
