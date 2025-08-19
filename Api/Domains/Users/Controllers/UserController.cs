using Api.Domains.Users.DTOs;
using Api.Domains.Users.Models;
using Api.Domains.Users.Repository;
using Microsoft.AspNetCore.Mvc;

namespace Api.Domains.Users.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository userDATA;

        public UserController(IUserRepository usuario)
        {
            userDATA = usuario;
        }

        [HttpGet]
        public IActionResult ListarUsers()
        {
            var listado = userDATA.GetList();
            return Ok(listado);
        }

        [HttpGet("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            var search = userDATA.GetById(id);
            if (search == null)
                return NotFound("Usuario no encontrado");

            return Ok(search);
        }

        [HttpPost]
        public IActionResult Registrar([FromBody] UserDTO dto)
        {
            try
            {
                var affected = userDATA.Create(dto);
                if (affected <= 0)
                    return BadRequest("No se pudo crear el usuario");

                return Ok("Usuario creado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }


        [HttpPut("{id}")]
        public IActionResult UpdateUser(int id, [FromBody] UserDTO dto)
        {
            try
            {
                var affected = userDATA.Update(id, dto);
                if (affected <= 0)
                    return NotFound("Usuario no encontrado o no se pudo actualizar");

                return Ok("Usuario actualizado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            try
            {
                var affected = userDATA.Delete(id);
                if (affected <= 0)
                    return NotFound("Usuario no encontrado o no se pudo eliminar");

                return Ok("Usuario eliminado correctamente");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
