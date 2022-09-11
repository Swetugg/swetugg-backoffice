using System;
using System.Configuration;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Swetugg.Web.Controllers;

namespace Swetugg.Web.Services
{
    public class ImageUploader : IImageUploader
    {
        private readonly CloudStorageAccount _storageAccount;
        private readonly bool _isEnabled;

        public ImageUploader()
        {
            try
            {
                _storageAccount = CloudStorageAccount.Parse(
                    ConfigurationManager.ConnectionStrings["StorageConnection"].ConnectionString);
                _isEnabled = true;
            }
            catch (FormatException)
            {
                _isEnabled = false;
            }
        }

        public async Task<string> UploadToStorage(Stream imageStream, string fileName, string containerName)
        {
            Image parsedImage;
            try
            {
                parsedImage = Image.FromStream(imageStream);
            }
            catch (ArgumentException e)
            {
                throw new ImageUploadException("File is not an image. Please use either JPG or PNG.", e);
            }

            if (!(parsedImage.RawFormat.Equals(ImageFormat.Jpeg) || parsedImage.RawFormat.Equals(ImageFormat.Png) || parsedImage.RawFormat.Equals(ImageFormat.Gif)))
            {
                throw new ImageUploadException("Only JPG, PNG or GIF images are supported.");
            }
            if (!_isEnabled)
            {
                throw new ImageUploadException("Image uploading is disabled. Please set a valid StorageConnection string");
            }

            try
            {
                CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container =
                    blobClient.GetContainerReference(containerName);
                await container.CreateIfNotExistsAsync(BlobContainerPublicAccessType.Blob, null, null);

                var speakerFileName = fileName + "-" + Guid.NewGuid();

                CloudBlockBlob blockBlob = container.GetBlockBlobReference(speakerFileName);
                blockBlob.Properties.ContentType = parsedImage.RawFormat.GetMimeType();
                blockBlob.Properties.CacheControl = "max-age=31536000, public";
                imageStream.Seek(0, SeekOrigin.Begin);
                await blockBlob.UploadFromStreamAsync(imageStream);

                return blockBlob.Uri.ToString();
            }
            catch (Exception e)
            {
                throw new ImageUploadException("Image upload failed", e);
            }
        }

        public async Task DeleteImage(string imageUrl)
        {
            try
            {
                CloudBlobClient blobClient = _storageAccount.CreateCloudBlobClient();

                var blob = await blobClient.GetBlobReferenceFromServerAsync(new Uri(imageUrl));
                await blob.DeleteAsync();
            }
            catch (Exception e)
            {
                throw new ImageUploadException("Image deletion failed", e);
            }
        }
    }
}