using Api.Repositories.Account;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Account.Requests;

namespace Api.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountRepository _account;

        public AccountController(IAccountRepository account)
        {
            _account = account;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var user = _account.Login(request);
            if (user == null)
                return Unauthorized("Correo o contraseña incorrectos");

            return Ok(user);
        }

        [HttpPut("{id}/profile")]
        public IActionResult UpdateProfile(int id, [FromBody] UpdateProfileRequest request)
        {
            try
            {
                var affected = _account.UpdateProfile(id, request);
                if (affected <= 0)
                    return NotFound("Usuario no encontrado o no se pudo actualizar el perfil");

                return Ok("Perfil actualizado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPut("{id}/profile-picture")]
        public IActionResult UpdateProfilePicture(int id, [FromBody] UpdateProfilePictureRequest request)
        {
            try
            {
                var affected = _account.UpdateProfilePicture(id, request);
                if (affected <= 0)
                    return NotFound("Usuario no encontrado o no se pudo actualizar la foto de perfil");

                return Ok("Foto de perfil actualizada correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpPut("{id}/password")]
        public IActionResult UpdatePassword(int id, [FromBody] UpdatePasswordRequest request)
        {
            try
            {
                var affected = _account.UpdatePassword(id, request);
                if (affected <= 0)
                    return NotFound("Usuario no encontrado o no se pudo actualizar la contraseña");

                return Ok("Contraseña actualizada correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
