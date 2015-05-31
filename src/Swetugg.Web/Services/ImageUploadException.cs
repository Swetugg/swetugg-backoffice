using System;
using System.Runtime.Serialization;

namespace Swetugg.Web.Services
{
    [Serializable]
    public class ImageUploadException : Exception
    {
        public ImageUploadException()
        {
        }

        public ImageUploadException(string message) : base(message)
        {
        }

        public ImageUploadException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ImageUploadException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}