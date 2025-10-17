using Api.Repositories.Users;
using Microsoft.AspNetCore.Mvc;
using Shared.DTOs.Users.Requests;

namespace Api.Controllers
{
    [Route("api/users")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserRepository _users;

        public UserController(IUserRepository users)
        {
            _users = users;
        }

        [HttpGet]
        public IActionResult GetList([FromQuery] int id)
        {
            var users = _users.GetList(id);
            return Ok(users);
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var user = _users.GetById(id);
            if (user == null)
                return NotFound();

            return Ok(user);
        }

        [HttpPost]
        public IActionResult Create([FromBody] CreateUserRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (newId, error) = _users.Create(dto);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(new { message = error });

            if (newId > 0)
            {
                var createdUser = _users.GetById(newId);
                return CreatedAtAction(nameof(GetById), new { id = newId }, createdUser);
            }

            return BadRequest("No se pudo crear el usuario");
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UpdateUserRequest dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var (affectedRows, error) = _users.Update(id, dto);

            if (!string.IsNullOrEmpty(error))
                return BadRequest(new { message = error });

            if (affectedRows > 0)
            {
                var updatedUser = _users.GetById(id);
                return Ok(updatedUser);
            }

            return NotFound("Usuario no encontrado o no se pudo actualizar");
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                var affected = _users.Delete(id);
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
