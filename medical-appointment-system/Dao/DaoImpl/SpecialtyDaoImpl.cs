using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using medical_appointment_system.Models;
using medical_appointment_system.Helpers;
using System;

namespace medical_appointment_system.Dao.DaoImpl
{
    public class SpecialtyDaoImpl : IGenericDao<Specialty>
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string crudCommand = "Specialty_CRUD";

        public int ExecuteWrite(string indicator, Specialty s)
        {
            int process = -1;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, s);

                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
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

        public List<Specialty> ExecuteRead(string indicator, Specialty s)
        {
            List<Specialty> list = new List<Specialty>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, s);

                    using (SqlDataReader reader = cmd.ExecuteReader())
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