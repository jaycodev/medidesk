using System.Data;
using Api.Data.Contract;
using Api.Data.Repository;
using Api.Domains.Users.DTOs;
using Api.Domains.Users.Models;
using Api.Helpers;
using Microsoft.Data.SqlClient;

namespace Api.Domains.Users.Repository
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        string crudCommand = "User_CRUD";

        public UserRepository(IConfiguration configuration) : base(configuration) { }

        public int Create(UserDTO dto)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "INSERT");
            cmd.Parameters.AddWithValue("@first_name", dto.FirstName);
            cmd.Parameters.AddWithValue("@last_name", dto.LastName);
            cmd.Parameters.AddWithValue("@email", dto.Email);
            cmd.Parameters.AddWithValue("@password", dto.Password);
            cmd.Parameters.AddWithValue("@phone", dto.Phone);

            // Devuelve las filas afectadas desde el procedimiento
            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int Delete(int id)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "DELETE");
            cmd.Parameters.AddWithValue("@user_id", id);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public User GetById(int id)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_ID");
            cmd.Parameters.AddWithValue("@user_id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new User
                {
                    UserId = reader.SafeGetInt("user_id"),
                    FirstName = reader.SafeGetString("first_name"),
                    LastName = reader.SafeGetString("last_name"),
                    Email = reader.SafeGetString("email"),
                    Phone = reader.SafeGetString("phone"),
                    Roles = reader.SafeGetString("roles")?.Split(',').ToList(),
                    ProfilePicture = reader.SafeGetString("profile_picture")
                };
            }

            return null;
        }

        public List<User> GetList()
        {
            var list = new List<User>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new User
               {
                UserId = reader.SafeGetInt("user_id"),
                FirstName = reader.SafeGetString("first_name"),
                LastName = reader.SafeGetString("last_name"),
                Email = reader.SafeGetString("email"),
                Phone = reader.SafeGetString("phone"),
                Roles = reader.SafeGetString("roles")?.Split(',').ToList()
               });
            }
           return list;     
        }



        public int Update(int id, UserDTO dto)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "UPDATE");
            cmd.Parameters.AddWithValue("@user_id", id);
            cmd.Parameters.AddWithValue("@first_name", dto.FirstName);
            cmd.Parameters.AddWithValue("@last_name", dto.LastName);
            cmd.Parameters.AddWithValue("@email", dto.Email);
            cmd.Parameters.AddWithValue("@phone", dto.Phone);

            // Si quieres pasar roles: cmd.Parameters.AddWithValue("@roles", "administrador,medico");

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

    }
}
