using System.Data;
using Api.Domains.Patients.DTOs;
using Api.Domains.Patients.Models;
using Api.Domains.Patients.Repositories;
using Api.Helpers;
using Microsoft.Data.SqlClient;

namespace Api.Data.Repository
{
    public class PatientRepository : BaseRepository, IPatientRepository
    {
        string crudCommand = "Patient_CRUD";

        public PatientRepository(IConfiguration configuration) : base(configuration) { }

        public List<PatientListDTO> GetList()
        {
            var list = new List<PatientListDTO>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new PatientListDTO
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

        public PatientDetailDTO? GetById(int id)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_ID");
            cmd.Parameters.AddWithValue("@user_id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new PatientDetailDTO
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
        public (int newId, string? error) Create(CreatePatientDTO dto)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "INSERT");
            cmd.Parameters.AddWithValue("@first_name", dto.FirstName);
            cmd.Parameters.AddWithValue("@last_name", dto.LastName);
            cmd.Parameters.AddWithValue("@email", dto.Email);
            cmd.Parameters.AddWithValue("@password", dto.Password);
            cmd.Parameters.AddWithValue("@phone", dto.Phone);
            cmd.Parameters.AddWithValue("@birth_date", dto.BirthDate);
            cmd.Parameters.AddWithValue("@blood_type", dto.BloodType);

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

        public int Update(int id, UpdatePatientDTO dto)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@indicator", "UPDATE");
            cmd.Parameters.AddWithValue("@user_id", id);
            cmd.Parameters.AddWithValue("@birth_date", dto.BirthDate);
            cmd.Parameters.AddWithValue("@blood_type", dto.BloodType);

            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? -1 : Convert.ToInt32(result);
        }
    }
}
