using System.ComponentModel.DataAnnotations;

namespace Api.Domains.CloudinaryImplement.DTOs
{
    public class UploadImageRequest
    {
        public IFormFile File { get; set; }
        public string Folder { get; set; }
        public string FileName { get; set; }
    }
}
