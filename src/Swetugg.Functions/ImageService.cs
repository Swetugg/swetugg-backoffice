using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Shared.Helpers;

namespace Swetugg.Functions
{
    public class ImageService
    {
        private readonly ILogger _logger;
        private readonly IImageHelper _imageHelper;

        public ImageService(ILoggerFactory loggerFactory, IImageHelper imageHelper)
        {
            _logger = loggerFactory.CreateLogger<ImageService>();
            _imageHelper = imageHelper;
        }

        /// <summary>
        /// Resizes an image based on the provided width, height, and padding options.
        /// The image will always be returned as a webp.
        /// </summary>
        /// <param name="req">The HTTP request data.</param>
        /// <returns>The resized image as an HTTP response.</returns>
        [Function("ResizeImage")]
        public async Task<HttpResponseData> ResizeImage([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation($"HTTP triggered ResizeImage.");

            // Having argumentnull exceptions here is a bit weird, but they are query variables that are required.
            int width = Int32.Parse(req.Query["width"] ?? throw new ArgumentNullException(nameof(width)));
            int height = Int32.Parse(req.Query["height"] ?? throw new ArgumentNullException(nameof(height)));
            bool pad = bool.Parse(req.Query["pad"] ?? "false");
            var origImage = ReadAllBytes(req.Body);

            var resizedImage = await _imageHelper.ResizeImage(origImage, width, height, pad);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "image/webp");
            await response.WriteBytesAsync(resizedImage);

            _logger.LogInformation($"Image resized to {width}x{height} and returned.");

            return response;
        }

        [Function("SetFormat")]
        public async Task<HttpResponseData> SetFormat([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation($"HTTP triggered SetFormat.");

            ImageFormat format = Enum.Parse<ImageFormat>(req.Query["format"] ?? throw new ArgumentNullException(nameof(format)));
            var origImage = ReadAllBytes(req.Body);

            var formattedImage = await _imageHelper.SetFormat(origImage, ImageFormat.WebP);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", $"image/{format}");
            await response.WriteBytesAsync(formattedImage);

            _logger.LogInformation($"Image reformatted to {format} and returned.");

            return response;
        }

        public byte[] ReadAllBytes(Stream stream)
        {
            using (var ms = new MemoryStream())
            {
                stream.CopyTo(ms);
                return ms.ToArray();
            }
        }
    }
}
