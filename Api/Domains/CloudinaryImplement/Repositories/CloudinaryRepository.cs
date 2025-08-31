using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace Api.Domains.CloudinaryImplement.Repositories
{
    public class CloudinaryRepository : ICloudinaryRepository
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryRepository(IConfiguration configuration)
        {
            var cloudName = configuration["Cloudinary:CloudName"];
            var apiKey = configuration["Cloudinary:ApiKey"];
            var apiSecret = configuration["Cloudinary:ApiSecret"];

            var account = new Account(cloudName, apiKey, apiSecret);
            _cloudinary = new Cloudinary(account);
        }

        public async Task<string> UploadImageAsync(string rutaArchivo, string nameFolder, string nameFile)
        {
            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(rutaArchivo),
                PublicId = $"{nameFolder}/{nameFile}",
                Overwrite = true
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
