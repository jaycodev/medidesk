using Api.Data.Contract;
using Api.Data.Repository;
using Api.Models;
using Api.Models.DTOS;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGenericContract<User> userDATA;


        public UserController(IGenericContract<User> usuario)
        {
            userDATA = usuario;
        }

        [HttpGet]
        public IActionResult ListarUsers()
        {
            User user = new User();
            return Ok(userDATA.ExecuteRead("GET_ALL", user));
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult ObtenerPorId(int id)
        {
            User user = new User{UserId = id,};
            var cliente = userDATA.ExecuteRead("GET_BY_ID", user);
            if (cliente == null)
                return NotFound();

            return Ok(cliente);
        }

        [HttpPost]
        public IActionResult Registrar(UserDTO dto)
        {
            User register = new User
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Password = dto.Password,
                Phone = dto.Phone
            };

            var result = userDATA.ExecuteWrite("INSERT", register);

            if (result > 0)
                return Ok(new { message = "Usuario creado correctamente" });

            return BadRequest("No se pudo crear el usuario");
        }

        [HttpPut]
        public IActionResult UpdateUser(int id, UserUpdateDTO dto)
        {

            var user = new User
            {
                UserId = id,
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Phone = dto.Phone,
            };

            var result = userDATA.ExecuteWrite("UPDATE", user);

            if (result > 0)
                return Ok(new { message = "Usuario actualizado correctamente" });

            return NotFound("Usuario no encontrado o no se pudo actualizar");
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteUser(int id)
        {
            
            var user = new User
            {
                UserId = id,
            };

            var result = userDATA.ExecuteWrite("DELETE", user);

            if (result > 0)
                return Ok(new { message = "Usuario actualizado correctamente" });

            return NotFound("Usuario no encontrado o no se pudo actualizar");
        }


    }
}
