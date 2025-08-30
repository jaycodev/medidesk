using CloudinaryDotNet.Actions;
using CloudinaryDotNet;

namespace Api.Domains.CloudinaryImplement.Repositories
{
    public interface ICloudinaryRepository
    {
        Task<string> UploadImageAsync(string rutaArchivo, string nameFolder, string nameFile);

        Task<bool> DeleteImageAsync(string publicId);

        Task<List<string>> GetImagesFromFolderAsync(string carpeta);
    }
}
