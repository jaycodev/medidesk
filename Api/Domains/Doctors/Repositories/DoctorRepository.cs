using System.Data;
using Api.Data.Repository;
using Api.Domains.Doctors.DTOs;
using Api.Helpers;
using Microsoft.Data.SqlClient;

namespace Api.Domains.Doctors.Repositories
{
    public class DoctorRepository : BaseRepository, IDoctorRepository
    {
        string crudCommand = "Doctor_CRUD";

        public DoctorRepository(IConfiguration configuration) : base(configuration) { }

        public List<DoctorListDto> GetList()
        {
            var list = new List<DoctorListDto>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new DoctorListDto
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

        public DoctorDetailDto? GetById(int id)
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
                return new DoctorDetailDto
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
        public (int newId, string? error) Create(CreateDoctorDto dto)
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
            cmd.Parameters.AddWithValue("@specialty_id", dto.SpecialtyId);

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

        public int Update(int id, UpdateDoctorDto dto)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@indicator", "UPDATE");
            cmd.Parameters.AddWithValue("@user_id", id);
            cmd.Parameters.AddWithValue("@specialty_id", dto.SpecialtyId);
            cmd.Parameters.AddWithValue("@status", dto.Status);

            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? -1 : Convert.ToInt32(result);
        }
    }
}
