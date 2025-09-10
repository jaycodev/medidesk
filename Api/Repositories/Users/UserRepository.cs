using System.Data;
using Api.Helpers;
using Microsoft.Data.SqlClient;
using Shared.DTOs.Users.Requests;
using Shared.DTOs.Users.Responses;

namespace Api.Repositories.Users
{
    public class UserRepository : BaseRepository, IUserRepository
    {
        private const string CrudCommand = "User_CRUD";

        public UserRepository(string connectionString) : base(connectionString) { }

        public List<UserListResponse> GetList(int id)
        {
            var list = new List<UserListResponse>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");
            cmd.Parameters.AddWithValue("@user_id", id);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new UserListResponse
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
                    ProfilePicture = reader.SafeGetString("profile_picture"),
                    CanDelete = reader.SafeGetBool("can_delete")
                });
            }

            return list;
        }

        public UserResponse? GetById(int id)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_ID");
            cmd.Parameters.AddWithValue("@user_id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new UserResponse
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
                    ProfilePicture = reader.SafeGetString("profile_picture"),

                    SpecialtyName = reader.SafeGetString("specialty_name"),
                    Status = reader.SafeGetBool("status"),

                    BirthDate = reader.SafeGetDateOnly("birth_date"),
                    BloodType = reader.SafeGetString("blood_type")
                };
            }

            return null;
        }

        public (int newId, string? error) Create(CreateUserRequest request)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "INSERT");
            cmd.Parameters.AddWithValue("@first_name", request.FirstName);
            cmd.Parameters.AddWithValue("@last_name", request.LastName);
            cmd.Parameters.AddWithValue("@email", request.Email);
            cmd.Parameters.AddWithValue("@password", request.Password);
            cmd.Parameters.AddWithValue("@phone", request.Phone);

            var paramNewId = new SqlParameter("@new_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var paramError = new SqlParameter("@error_message", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(paramNewId);
            cmd.Parameters.Add(paramError);

            try
            {
                cmd.ExecuteNonQuery();

                int newId = paramNewId.Value == DBNull.Value ? -1 : Convert.ToInt32(paramNewId.Value);
                string? error = paramError.Value == DBNull.Value ? null : Convert.ToString(paramError.Value);

                return (newId, error);
            }
            catch (SqlException ex)
            {
                var sqlMessage = ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message;
                throw new InvalidOperationException(sqlMessage, ex);
            }
        }

        public (int affectedRows, string? error) Update(int id, UpdateUserRequest request)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "UPDATE");
            cmd.Parameters.AddWithValue("@user_id", id);
            cmd.Parameters.AddWithValue("@first_name", request.FirstName);
            cmd.Parameters.AddWithValue("@last_name", request.LastName);
            cmd.Parameters.AddWithValue("@email", request.Email);
            cmd.Parameters.AddWithValue("@phone", request.Phone);
            cmd.Parameters.AddWithValue("@roles", string.Join(",", request.Roles));

            var paramError = new SqlParameter("@error_message", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(paramError);

            try
            {
                var result = cmd.ExecuteScalar();

                int affectedRows = result != null && result != DBNull.Value
                    ? Convert.ToInt32(result)
                    : 0;

                string? error = paramError.Value == DBNull.Value ? null : Convert.ToString(paramError.Value);

                return (affectedRows, error);
            }
            catch (SqlException ex)
            {
                var sqlMessage = ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message;
                throw new InvalidOperationException(sqlMessage, ex);
            }
        }

        public int Delete(int id)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.AddWithValue("@indicator", "DELETE");
            cmd.Parameters.AddWithValue("@user_id", id);

            var result = cmd.ExecuteScalar();
            return result != null ? Convert.ToInt32(result) : 0;
        }
    }
}
