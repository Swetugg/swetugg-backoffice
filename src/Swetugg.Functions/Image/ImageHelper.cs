using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Processing;
using Swetugg.Shared.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ImageSharp = SixLabors.ImageSharp;

namespace Swetugg.Functions.Image;
internal class ImageHelper : IImageHelper
{
    public async Task<byte[]> ResizeImage(byte[] image, int? width, int? height, bool pad)
    {
        using var img = ImageSharp.Image.Load(image);
        using var outStream = new MemoryStream();

        img.Mutate(i => i.Resize(new ResizeOptions
        {
            Size = new ImageSharp.Size(width ?? 0, height ?? 0),
            Mode = pad ? ResizeMode.Pad : ResizeMode.Stretch
        }));

        await img.SaveAsync(outStream, new ImageSharp.Formats.Webp.WebpEncoder());

        outStream.Seek(0, SeekOrigin.Begin);
        return outStream.ToArray();
    }

    public async Task<byte[]> SetFormat(byte[] image, ImageFormat format)
    {
        using var img = ImageSharp.Image.Load(image);
        using var outStream = new MemoryStream();

        IImageEncoder encoder = format switch
        {
            ImageFormat.Jpeg => new ImageSharp.Formats.Jpeg.JpegEncoder(),
            ImageFormat.Png => new ImageSharp.Formats.Png.PngEncoder(),
            ImageFormat.WebP => new ImageSharp.Formats.Webp.WebpEncoder(),
            _ => throw new ArgumentException($"Unsupported image format {format}", nameof(format))
        };

        await img.SaveAsync(outStream, encoder);

        outStream.Seek(0, SeekOrigin.Begin);
        return outStream.ToArray();
    }
}
