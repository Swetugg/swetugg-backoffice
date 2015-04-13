using System.Drawing.Imaging;
using System.Linq;

namespace Swetugg.Web.Controllers
{
    public static class ImageExtensions
    {
        public static string GetMimeType(this ImageFormat imageFormat)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            return codecs.First(codec => codec.FormatID == imageFormat.Guid).MimeType;
        }
    }
}