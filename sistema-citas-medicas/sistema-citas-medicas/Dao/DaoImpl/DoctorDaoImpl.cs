using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using sistema_citas_medicas.Helpers;
using sistema_citas_medicas.Models;

namespace sistema_citas_medicas.Dao.DaoImpl
{
    public class DoctorDaoImpl : IGenericDao<Doctor>
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string crudCommand = "Doctor_CRUD";

        public int ExecuteWrite(string indicator, Doctor d)
        {
            int process = -1;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, d);

                    try
                    {
                        process = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        if (ex.Number == 50000)
                        {
                            throw new ApplicationException(ex.Message);
                        }
                        else
                        {
                            throw;
                        }
                    }
                }
            }
            return process;
        }

        public List<Doctor> ExecuteRead(string indicator, Doctor d)
        {
            List<Doctor> list = new List<Doctor>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, d);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Doctor
                            {
                                UserId = reader.SafeGetInt("user_id"),
                                FirstName = reader.SafeGetString("first_name"),
                                LastName = reader.SafeGetString("last_name"),
                                Email = reader.SafeGetString("email"),
                                Password = reader.SafeGetString("password"),
                                Phone = reader.SafeGetString("phone"),
                                ProfilePicture = reader.SafeGetString("profile_picture"),
                                Role = reader.SafeGetString("role"),
                                SpecialtyId = reader.SafeGetInt("specialty_id"),
                                SpecialtyName = reader.SafeGetString("specialty_name"),
                                Status = reader.SafeGetBool("status")
                            });
                        }
                    }
                }
            }
            return list;
        }

        private void AddParameters(SqlCommand cmd, string indicator, Doctor d)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@user_id", d.UserId);
            cmd.Parameters.AddWithValue("@first_name", d.FirstName);
            cmd.Parameters.AddWithValue("@last_name", d.LastName);
            cmd.Parameters.AddWithValue("@email", d.Email);
            cmd.Parameters.AddWithValue("@password", d.Password);
            cmd.Parameters.AddWithValue("@phone", d.Phone);
            cmd.Parameters.AddWithValue("@profile_picture", d.ProfilePicture);
            cmd.Parameters.AddWithValue("@role", d.Role);
            cmd.Parameters.AddWithValue("@specialty_id", d.SpecialtyId);
            cmd.Parameters.AddWithValue("@status", d.Status);
        }
    }
}
