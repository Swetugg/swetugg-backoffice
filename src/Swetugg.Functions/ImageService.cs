using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Swetugg.Shared.Helpers;
using System.Net;
using System.Runtime.CompilerServices;

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

            if (!Int32.TryParse(req.Query["width"], out var width))
            {
                return MissingArgumentError(req, nameof(width));
            }
            if (!Int32.TryParse(req.Query["height"], out var height))
            {
                return MissingArgumentError(req, nameof(height));
            }

            bool pad = bool.Parse(req.Query["pad"] ?? "false");
            var origImage = ReadAllBytes(req.Body);

            var resizedImage = await _imageHelper.ResizeImage(origImage, width, height, pad);

            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "image/webp");
            await response.WriteBytesAsync(resizedImage);

            _logger.LogInformation($"Image resized to {width}x{height} and returned.");

            return response;
        }

        /// <summary>
        /// Sets the format of an image to the specified format.
        /// </summary>
        /// <param name="req">The HTTP request data.</param>
        /// <returns>The formatted image as an HTTP response.</returns>
        [Function("SetFormat")]
        public async Task<HttpResponseData> SetFormat([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequestData req)
        {
            _logger.LogInformation($"HTTP triggered SetFormat.");

            if (!Enum.TryParse<ImageFormat>(req.Query["format"], out var format))
            {
                return MissingArgumentError(req, nameof(format));
            }
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

        private HttpResponseData MissingArgumentError(HttpRequestData req, string argumentName, [CallerMemberName] string? caller = null)
        {
            _logger.LogInformation($"{caller}: Missing or invalid required parameter '{argumentName}'");
            var resp = req.CreateResponse(HttpStatusCode.BadRequest);
            resp.WriteString($"Missing or invalid required parameter '{argumentName}'");
            return resp;
        }
    }
}
