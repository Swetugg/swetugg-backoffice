using System.IO;
using Microsoft.Ajax.Utilities;

namespace Swetugg.Web.Services
{
    public interface IImageUploader
    {
        string UploadToStorage(Stream imageStream, string fileName, string containerName);
        void DeleteImage(string imageUrl);
    }
}