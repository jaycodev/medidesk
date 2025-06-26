using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using medical_appointment_system.Models;
using medical_appointment_system.Services.Interfaces;

namespace medical_appointment_system.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService()
        {
            var config = CloudinarySettings.FromConfig();
            var account = new Account(config.CloudName, config.ApiKey, config.ApiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(string rutaArchivo, string nameFolder, string nameFile)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(rutaArchivo),
                PublicId = $"{nameFolder}/{nameFile}",
                Overwrite = true // opcional, útil si quieres reemplazar imágenes con mismo nombre
            };

            var result = await _cloudinary.UploadAsync(uploadParams);
            return result.SecureUrl.ToString();
        }

        public async Task<bool> DeleteImageAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);
            return result.Result == "ok";
        }

        public async Task<List<string>> GetImagesFromFolderAsync(string carpeta)
        {
            var resultado = new List<string>();
            string nextCursor = null;

            do
            {
                // Construir y ejecutar búsqueda directamente
                var search = _cloudinary.Search()
                    .Expression($"folder:{carpeta}")
                    .SortBy("public_id", "asc")
                    .MaxResults(100);

                if (!string.IsNullOrEmpty(nextCursor))
                    search = search.NextCursor(nextCursor);

                var searchResult = await search.ExecuteAsync();

                resultado.AddRange(searchResult.Resources.Select(r => r.SecureUrl.ToString()));
                nextCursor = searchResult.NextCursor;

            } while (!string.IsNullOrEmpty(nextCursor));

            return resultado;
        }

    }
}