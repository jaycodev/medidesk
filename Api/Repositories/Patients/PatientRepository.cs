using System.Data;
using Api.Helpers;
using Microsoft.Data.SqlClient;
using Shared.DTOs.Patients.Requests;
using Shared.DTOs.Patients.Responses;

namespace Api.Repositories.Patients
{
    public class PatientRepository : BaseRepository, IPatientRepository
    {
        private const string CrudCommand = "Patient_CRUD";

        public PatientRepository(IConfiguration configuration) : base(configuration) { }

        public List<PatientListResponse> GetList()
        {
            var list = new List<PatientListResponse>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new PatientListResponse
                {
                    UserId = reader.SafeGetInt("user_id"),
                    FirstName = reader.SafeGetString("first_name"),
                    LastName = reader.SafeGetString("last_name"),
                    Email = reader.SafeGetString("email"),
                    ProfilePicture = reader.SafeGetString("profile_picture"),
                    BirthDate = reader.SafeGetDateOnly("birth_date"),
                    BloodType = reader.SafeGetString("blood_type")
                });
            }

            return list;
        }

        public PatientResponse? GetById(int id)
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
                return new PatientResponse
                {
                    UserId = reader.SafeGetInt("user_id"),
                    FirstName = reader.SafeGetString("first_name"),
                    LastName = reader.SafeGetString("last_name"),
                    Email = reader.SafeGetString("email"),
                    Phone = reader.SafeGetString("phone"),
                    ProfilePicture = reader.SafeGetString("profile_picture"),
                    BirthDate = reader.SafeGetDateOnly("birth_date"),
                    BloodType = reader.SafeGetString("blood_type")
                };
            }

            return null;
        }

        public (int newId, string? error) Create(CreatePatientRequest request)
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
            cmd.Parameters.AddWithValue("@birth_date", request.BirthDate);
            cmd.Parameters.AddWithValue("@blood_type", request.BloodType);

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

        public int Update(int id, UpdatePatientRequest request)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@indicator", "UPDATE");
            cmd.Parameters.AddWithValue("@user_id", id);
            cmd.Parameters.AddWithValue("@birth_date", request.BirthDate);
            cmd.Parameters.AddWithValue("@blood_type", request.BloodType);

            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? -1 : Convert.ToInt32(result);
        }
    }
}
