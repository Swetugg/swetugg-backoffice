using Swetugg.Shared.Services;
using System.Net;
using Swetugg.Shared.Tests.Fakes;
using Swetugg.Shared.Helpers;

namespace Swetugg.Shared.Tests
{
    public class ImageHelperTests
    {
        private readonly string _baseUrl = "http://localhost:5000";

        [Theory]
        public async Task Executing_ResizeImage_CallsHttpClientCorrectly()
        {
            // Arrange
            var random = new Random();
            byte[] image = new byte[100];
            random.NextBytes(image);

            string? calledUrl = null;
            var client = new HttpClient(
                new HttpMessageHandlerFake(
                    (message, _) =>
                    {
                        calledUrl = message?.RequestUri?.ToString();
                        var response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(image)
                        };
                        return Task.FromResult(response);
                    })
                );

            int? width = 100;
            int? height = 100;
            bool pad = false;

            var imageHelper = new ImageHelper(client, _baseUrl);

            // Act
            var result = await imageHelper.ResizeImage(image, width, height, pad);

            // Assert

            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo(image));
            Assert.That(calledUrl, Is.EqualTo($"{_baseUrl}/resize?width={width}&height={height}&pad={pad}"));
        }
        
        [Theory]
        [TestCase(ImageFormat.Jpeg)]
        [TestCase(ImageFormat.Png)]
        [TestCase(ImageFormat.WebP)]
        public async Task Executing_SetFormat_CallsHttpClientCorrectly(ImageFormat format)
        {
            // Arrange
            var random = new Random();
            byte[] image = new byte[100];
            random.NextBytes(image);

            string? calledUrl = null;
            var client = new HttpClient(
                new HttpMessageHandlerFake(
                    (message, _) =>
                    {
                        calledUrl = message?.RequestUri?.ToString();
                        var response = new HttpResponseMessage(HttpStatusCode.OK)
                        {
                            Content = new ByteArrayContent(image)
                        };
                        return Task.FromResult(response);
                    })
                );



            var imageHelper = new ImageHelper(client, _baseUrl);

            // Act
            var result = await imageHelper.SetFormat(image, format);

            // Assert
            Assert.NotNull(result);
            Assert.That(result, Is.EqualTo(image));
            Assert.That(calledUrl, Is.EqualTo($"{_baseUrl}/format?format={format}"));
        }
    }
}
