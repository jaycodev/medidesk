namespace Api.Repositories.Cloudinary
{
    public interface ICloudinaryRepository
    {
        Task<string> UploadImageAsync(string rutaArchivo, string nameFolder, string nameFile);
        Task<bool> DeleteImageAsync(string publicId);
        Task<List<string>> GetImagesFromFolderAsync(string carpeta);
    }
}
