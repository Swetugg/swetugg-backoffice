using Swetugg.Shared.Helpers;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Swetugg.Shared.Services;

///  <inheritdoc />
internal class ImageHelper(HttpClient httpClient, string baseUrl) : IImageHelper
{
    // Since this is currently in a .net standard library and accessible by both framework and net8.0, we can't use the HttpClientFactory
    // Once we migrate this to a .net8 library, we can use the HttpClientFactory

    /// <inheritdoc/>
    public async Task<byte[]> ResizeImage(byte[] image, int? width, int? height, bool pad)
    {
        // Make a request to the image resizing service
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        var url = $"{baseUrl}/resize?width={width}&height={height}&pad={pad}";
        var response = await httpClient.PostAsync(url, new ByteArrayContent(image));
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to resize image. Status code: {response.StatusCode}");
        }
        return await response.Content.ReadAsByteArrayAsync();
    }

    /// <inheritdoc/>
    public async Task<byte[]> SetFormat(byte[] image, ImageFormat format)
    {
        // Make a request to the image format conversion service
        if (image == null)
        {
            throw new ArgumentNullException(nameof(image));
        }

        var url = $"{baseUrl}/format?format={format}";
        var response = await httpClient.PostAsync(url, new ByteArrayContent(image));

        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Failed to resize image. Status code: {response.StatusCode}");
        }
        return await response.Content.ReadAsByteArrayAsync();
    }
}
