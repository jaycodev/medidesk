using Api.Data.Contract;
using Api.Helpers;
using Api.Models;
using Microsoft.Data.SqlClient;

namespace Api.Data.Repository
{
    public class SpecialtyRepository : BaseRepository, IGenericContract<Specialty>
    {
        string crudCommand = "Specialty_CRUD";

        public SpecialtyRepository(IConfiguration configuration) : base(configuration) { }

        public List<Specialty> ExecuteRead(string indicator, Specialty s)
        {
            List<Specialty> list = new List<Specialty>();

            using (var cn = GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, s);

                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Specialty
                            {
                                SpecialtyId = reader.SafeGetInt("specialty_id"),
                                Name = reader.SafeGetString("name"),
                                Description = reader.SafeGetString("description")
                            });
                        }
                    }
                }
            }
            return list;
        }

        public int ExecuteWrite(string indicator, Specialty s)
        {
            int process = -1;
            using (var cn = GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, s);

                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                process = Convert.ToInt32(reader["affected_rows"]);
                        }
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
            return process;
        }

        private void AddParameters(SqlCommand cmd, string indicator, Specialty s)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@specialty_id", s.SpecialtyId);
            cmd.Parameters.AddWithValue("@name", s.Name);
            cmd.Parameters.AddWithValue("@description", s.Description);
        }
    }
}
