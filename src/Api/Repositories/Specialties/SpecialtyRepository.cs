using System.Data;
using Api.Helpers;
using Microsoft.Data.SqlClient;
using Shared.DTOs.Specialties.Requests;
using Shared.DTOs.Specialties.Responses;

namespace Api.Repositories.Specialties
{
    public class SpecialtyRepository : BaseRepository, ISpecialtyRepository
    {
        private const string CrudCommand = "Specialty_CRUD";

        public SpecialtyRepository(string connectionString) : base(connectionString) { }

        public List<SpecialtyResponse> GetList()
        {
            var list = new List<SpecialtyResponse>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new SpecialtyResponse
                {
                    SpecialtyId = reader.SafeGetInt("specialty_id"),
                    Name = reader.SafeGetString("name"),
                    Description = reader.SafeGetString("description")
                });
            }

            return list;
        }

        public SpecialtyResponse? GetById(int id)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_ID");
            cmd.Parameters.AddWithValue("@specialty_id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new SpecialtyResponse
                {
                    SpecialtyId = reader.SafeGetInt("specialty_id"),
                    Name = reader.SafeGetString("name"),
                    Description = reader.SafeGetString("description")
                };
            }

            return null;
        }

        public int Create(SpecialtyRequest request)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "INSERT");
            cmd.Parameters.AddWithValue("@name", request.Name);
            cmd.Parameters.AddWithValue("@description", request.Description);

            var paramNewId = new SqlParameter("@new_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(paramNewId);

            cmd.ExecuteNonQuery();

            return paramNewId.Value == DBNull.Value ? -1 : Convert.ToInt32(paramNewId.Value);
        }

        public int Update(int id, SpecialtyRequest request)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@indicator", "UPDATE");
            cmd.Parameters.AddWithValue("@specialty_id", id);
            cmd.Parameters.AddWithValue("@name", request.Name);
            cmd.Parameters.AddWithValue("@description", request.Description);

            var paramAffected = new SqlParameter("@affected_rows", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(paramAffected);

            cmd.ExecuteNonQuery();

            return paramAffected.Value == DBNull.Value ? -1 : Convert.ToInt32(paramAffected.Value);
        }
    }
}
