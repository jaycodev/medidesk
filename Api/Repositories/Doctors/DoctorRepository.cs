using System.Data;
using Api.Helpers;
using Microsoft.Data.SqlClient;
using Shared.DTOs.Doctors.Requests;
using Shared.DTOs.Doctors.Responses;

namespace Api.Repositories.Doctors
{
    public class DoctorRepository : BaseRepository, IDoctorRepository
    {
        private const string CrudCommand = "Doctor_CRUD";

        public DoctorRepository(string connectionString) : base(connectionString) { }

        public List<DoctorListResponse> GetList()
        {
            var list = new List<DoctorListResponse>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new DoctorListResponse
                {
                    UserId = reader.SafeGetInt("user_id"),
                    FirstName = reader.SafeGetString("first_name"),
                    LastName = reader.SafeGetString("last_name"),
                    Email = reader.SafeGetString("email"),
                    SpecialtyName = reader.SafeGetString("specialty_name"),
                    ProfilePicture = reader.SafeGetString("profile_picture"),
                    Status = reader.SafeGetBool("status")
                });
            }

            return list;
        }

        public DoctorResponse? GetById(int id)
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
                return new DoctorResponse
                {
                    UserId = reader.SafeGetInt("user_id"),
                    FirstName = reader.SafeGetString("first_name"),
                    LastName = reader.SafeGetString("last_name"),
                    Email = reader.SafeGetString("email"),
                    Phone = reader.SafeGetString("phone"),
                    ProfilePicture = reader.SafeGetString("profile_picture"),
                    SpecialtyId = reader.SafeGetInt("specialty_id"),
                    SpecialtyName = reader.SafeGetString("specialty_name"),
                    Status = reader.SafeGetBool("status")
                };
            }

            return null;
        }

        public List<DoctorBySpecialtyResponse> GetBySpecialty(int specialtyId, int userId)
        {
            var list = new List<DoctorBySpecialtyResponse>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_SPECIALTY");
            cmd.Parameters.AddWithValue("@specialty_id", specialtyId);
            cmd.Parameters.AddWithValue("@user_id", userId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new DoctorBySpecialtyResponse
                {
                    UserId = reader.SafeGetInt("user_id"),
                    FirstName = reader.SafeGetString("first_name"),
                    LastName = reader.SafeGetString("last_name"),
                    SpecialtyId = reader.SafeGetInt("specialty_id")
                });
            }

            return list;
        }

        public (int newId, string? error) Create(CreateDoctorRequest request)
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
            cmd.Parameters.AddWithValue("@specialty_id", request.SpecialtyId);
            cmd.Parameters.AddWithValue("@status", request.Status);

            var paramNewId = new SqlParameter("@new_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            var paramError = new SqlParameter("@error_message", SqlDbType.NVarChar, 4000) { Direction = ParameterDirection.Output };

            cmd.Parameters.Add(paramNewId);
            cmd.Parameters.Add(paramError);

            try
            {
                cmd.ExecuteNonQuery();

                var newIdObj = paramNewId.Value;
                var errorObj = paramError.Value;

                int newId = newIdObj == DBNull.Value ? -1 : Convert.ToInt32(newIdObj);
                string? error = errorObj == DBNull.Value ? null : Convert.ToString(errorObj);

                return (newId, error);
            }
            catch (SqlException ex)
            {
                var sqlMessage = ex.Errors.Count > 0 ? ex.Errors[0].Message : ex.Message;
                throw new InvalidOperationException(sqlMessage, ex);
            }
        }

        public int Update(int id, UpdateDoctorRequest request)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@indicator", "UPDATE");
            cmd.Parameters.AddWithValue("@user_id", id);
            cmd.Parameters.AddWithValue("@specialty_id", request.SpecialtyId);
            cmd.Parameters.AddWithValue("@status", request.Status);

            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? -1 : Convert.ToInt32(result);
        }
    }
}
