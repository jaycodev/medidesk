using System.Data;
using Api.Data.Repository;
using Api.Domains.Specialties.DTOs;
using Api.Domains.Specialties.Models;
using Api.Helpers;
using Microsoft.Data.SqlClient;

namespace Api.Domains.Specialties.Repositories
{
    public class SpecialtyRepository : BaseRepository, ISpecialtyRepository
    {
        string crudCommand = "Specialty_CRUD";

        public SpecialtyRepository(IConfiguration configuration) : base(configuration) { }

        public List<Specialty> GetList()
        {
            var list = new List<Specialty>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Specialty
                {
                    SpecialtyId = reader.SafeGetInt("specialty_id"),
                    Name = reader.SafeGetString("name"),
                    Description = reader.SafeGetString("description")
                });
            }

            return list;
        }

        public Specialty? GetById(int id)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_ID");
            cmd.Parameters.AddWithValue("@specialty_id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new Specialty
                {
                    SpecialtyId = reader.SafeGetInt("specialty_id"),
                    Name = reader.SafeGetString("name"),
                    Description = reader.SafeGetString("description")
                };
            }

            return null;
        }

        public int Create(CreateSpecialtyDTO dto)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "INSERT");
            cmd.Parameters.AddWithValue("@name", dto.Name);
            cmd.Parameters.AddWithValue("@description", dto.Description);

            var paramNewId = new SqlParameter("@new_id", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(paramNewId);

            cmd.ExecuteNonQuery();

            return paramNewId.Value == DBNull.Value ? -1 : Convert.ToInt32(paramNewId.Value);
        }

        public int Update(int id, UpdateSpecialtyDTO dto)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@indicator", "UPDATE");
            cmd.Parameters.AddWithValue("@specialty_id", id);
            cmd.Parameters.AddWithValue("@name", dto.Name);
            cmd.Parameters.AddWithValue("@description", dto.Description);

            var paramAffected = new SqlParameter("@affected_rows", SqlDbType.Int) { Direction = ParameterDirection.Output };
            cmd.Parameters.Add(paramAffected);

            cmd.ExecuteNonQuery();

            return paramAffected.Value == DBNull.Value ? -1 : Convert.ToInt32(paramAffected.Value);
        }
    }
}
