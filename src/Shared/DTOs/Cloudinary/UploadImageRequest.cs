using Microsoft.AspNetCore.Http;

namespace Shared.DTOs.Cloudinary
{
    public class UploadImageRequest
    {
        public IFormFile? File { get; set; }
        public string Folder { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
    }
}
