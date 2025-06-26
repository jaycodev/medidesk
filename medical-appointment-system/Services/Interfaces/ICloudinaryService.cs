using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace medical_appointment_system.Services.Interfaces
{
    internal interface ICloudinaryService
    {
        Task<string> UploadImageAsync(string filePath, string folderName, string fileName);
        Task<bool> DeleteImageAsync(string publicId);
        Task<List<string>> GetImagesFromFolderAsync(string folderName);

    }
}
