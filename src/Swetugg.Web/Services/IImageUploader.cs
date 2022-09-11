using System.IO;
using System.Threading.Tasks;

namespace Swetugg.Web.Services
{
    public interface IImageUploader
    {
        Task<string> UploadToStorage(Stream imageStream, string fileName, string containerName);
        Task DeleteImage(string imageUrl);
    }
}