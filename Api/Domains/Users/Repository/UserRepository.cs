using System.Data;
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
                    Roles = reader.SafeGetString("roles")
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(r => r.Trim())
                                    .ToList(),
                    ProfilePicture = reader.SafeGetString("profile_picture")
                };
            }

            return null;
        }

        public List<User> GetList(int id)
        {
            var list = new List<User>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");
            cmd.Parameters.AddWithValue("@user_id", id);
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
                    Roles = reader.SafeGetString("roles")
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(r => r.Trim())
                                    .ToList(),
                    ProfilePicture = reader.SafeGetString("profile_picture")

                });
            }
            return list;
        }

        public int Update(int id, UserUpdateDTO dto)
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
            cmd.Parameters.AddWithValue("@roles", dto.SelectedRoleCombo);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public LoggedUserDTO? Login(string email, string password)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "LOGIN");
            cmd.Parameters.AddWithValue("@email", email);
            cmd.Parameters.AddWithValue("@password", password);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new LoggedUserDTO
            {
                UserId = reader.SafeGetInt("user_id"),
                FirstName = reader.SafeGetString("first_name"),
                LastName = reader.SafeGetString("last_name"),
                Email = reader.SafeGetString("email"),
                Roles = reader.SafeGetString("roles")
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(r => r.Trim())
                                    .ToList(),
                ProfilePicture = reader.SafeGetString("profile_picture"),
                Phone = reader.SafeGetString("phone")
            };
        }

        public int UpdatePassword(int userId, string newPassword,string currentPassword)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "UPDATE_PASSWORD");
            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@current_password", currentPassword);
            cmd.Parameters.AddWithValue("@password", newPassword);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int UpdateProfilePicture(int userId, string profilePictureUrl)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "UPDATE_PROFILE_PICTURE");
            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@profile_picture", profilePictureUrl);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int UpdateProfile(int userId, string phone)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "UPDATE_PROFILE");
            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@phone", phone);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}
