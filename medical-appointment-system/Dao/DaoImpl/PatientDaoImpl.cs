using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using medical_appointment_system.Helpers;
using medical_appointment_system.Models;

namespace medical_appointment_system.Dao.DaoImpl
{
    public class PatientDaoImpl : IGenericDao<Patient>
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string crudCommand = "Patient_CRUD";

        public int ExecuteWrite(string indicator, Patient p)
        {
            int process = -1;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, p);

                    try
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                process = Convert.ToInt32(reader["affected_rows"]);
                        }
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

        public List<Patient> ExecuteRead(string indicator, Patient p)
        {
            List<Patient> list = new List<Patient>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, p);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Patient
                            {
                                UserId = reader.SafeGetInt("user_id"),
                                FirstName = reader.SafeGetString("first_name"),
                                LastName = reader.SafeGetString("last_name"),
                                Email = reader.SafeGetString("email"),
                                Phone = reader.SafeGetString("phone"),
                                ProfilePicture = reader.SafeGetString("profile_picture"),
                                BirthDate = reader.SafeGetDateTime("birth_date"),
                                BloodType = reader.SafeGetString("blood_type")
                            });
                        }
                    }
                }
            }
            return list;
        }

        private void AddParameters(SqlCommand cmd, string indicator, Patient p)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@user_id", p.UserId);
            cmd.Parameters.AddWithValue("@first_name", p.FirstName);
            cmd.Parameters.AddWithValue("@last_name", p.LastName);
            cmd.Parameters.AddWithValue("@email", p.Email);
            cmd.Parameters.AddWithValue("@password", p.Password);
            cmd.Parameters.AddWithValue("@phone", p.Phone);
            cmd.Parameters.AddWithValue("@profile_picture", p.ProfilePicture);
            cmd.Parameters.AddWithValue("@birth_date", p.BirthDate);
            cmd.Parameters.AddWithValue("@blood_type", p.BloodType);
        }
    }
}