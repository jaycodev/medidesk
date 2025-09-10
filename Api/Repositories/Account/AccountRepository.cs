using System.Data;
using Api.Helpers;
using Microsoft.Data.SqlClient;
using Shared.DTOs.Account.Requests;
using Shared.DTOs.Account.Responses;

namespace Api.Repositories.Account
{
    public class AccountRepository : BaseRepository, IAccountRepository
    {
        private const string CrudCommand = "User_CRUD";

        public AccountRepository(string connectionString) : base(connectionString) { }

        public LoggedUserResponse? Login(LoginRequest request)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "LOGIN");
            cmd.Parameters.AddWithValue("@email", request.Email);
            cmd.Parameters.AddWithValue("@password", request.Password);

            using var reader = cmd.ExecuteReader();
            if (!reader.Read()) return null;

            return new LoggedUserResponse
            {
                UserId = reader.SafeGetInt("user_id"),
                FirstName = reader.SafeGetString("first_name"),
                LastName = reader.SafeGetString("last_name"),
                Email = reader.SafeGetString("email"),
                Roles = reader.SafeGetString("roles")
                                    .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                                    .Select(r => r.Trim())
                                    .ToList(),
                Phone = reader.SafeGetString("phone"),
                ProfilePicture = reader.SafeGetString("profile_picture")
            };
        }

        public int UpdateProfile(int userId, UpdateProfileRequest request)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "UPDATE_PROFILE");
            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@phone", request.Phone);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int UpdateProfilePicture(int id, UpdateProfilePictureRequest request)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "UPDATE_PROFILE_PICTURE");
            cmd.Parameters.AddWithValue("@user_id", id);
            cmd.Parameters.AddWithValue("@profile_picture", request.ProfilePictureUrl);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }

        public int UpdatePassword(int id, UpdatePasswordRequest request)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "UPDATE_PASSWORD");
            cmd.Parameters.AddWithValue("@user_id", id);
            cmd.Parameters.AddWithValue("@current_password", request.CurrentPassword);
            cmd.Parameters.AddWithValue("@password", request.NewPassword);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}
