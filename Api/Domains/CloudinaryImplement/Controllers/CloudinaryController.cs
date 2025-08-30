using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Api.Domains.CloudinaryImplement.Repositories;
using Api.Domains.CloudinaryImplement.DTOs;

namespace Api.Domains.CloudinaryImplement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CloudinaryController : ControllerBase
    {
        private readonly ICloudinaryRepository _cloudinaryRepository;

        public CloudinaryController(ICloudinaryRepository cloudinaryRepository)
        {
            _cloudinaryRepository = cloudinaryRepository;
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadImageRequest request)
        {
            if (request.File == null || request.File.Length == 0)
                return BadRequest("No se ha proporcionado ningún archivo.");

            try
            {
                var tempFilePath = Path.GetTempFileName();
                using (var stream = System.IO.File.Create(tempFilePath))
                {
                    await request.File.CopyToAsync(stream);
                }

                var url = await _cloudinaryRepository.UploadImageAsync(tempFilePath, request.Folder, request.FileName);

                System.IO.File.Delete(tempFilePath);

                return Ok(new { Url = url });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }



        [HttpDelete("delete/{publicId}")]
        public async Task<IActionResult> Delete(string publicId)
        {
            try
            {
                var eliminado = await _cloudinaryRepository.DeleteImageAsync(publicId);
                if (!eliminado)
                    return NotFound(new { Message = "No se pudo eliminar la imagen, verifique el publicId." });

                return Ok(new { Message = "Imagen eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }

        [HttpGet("list/{folder}")]
        public async Task<IActionResult> GetImages(string folder)
        {
            try
            {
                var imagenes = await _cloudinaryRepository.GetImagesFromFolderAsync(folder);
                return Ok(imagenes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Error = ex.Message });
            }
        }
    }
}
