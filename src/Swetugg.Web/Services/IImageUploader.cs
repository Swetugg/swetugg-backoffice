using System.IO;


namespace Swetugg.Web.Services
{
    public interface IImageUploader
    {
        string UploadToStorage(Stream imageStream, string fileName, string containerName);
        void DeleteImage(string imageUrl);
    }
}