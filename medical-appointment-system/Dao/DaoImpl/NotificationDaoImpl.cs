using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using medical_appointment_system.Helpers;
using medical_appointment_system.Models;

namespace medical_appointment_system.Dao.DaoImpl
{
    public class NotificationDaoImpl : IGenericDao<Notification>
    {

        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string crudCommand = "Notification_CRUD";

        public List<Notification> ExecuteRead(string indicator, Notification n)
        {
            List<Notification> list = new List<Notification>();
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, n);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Notification
                            {
                                NotificationId = reader.SafeGetInt("NotificationId"),
                                DoctorId = reader.SafeGetInt("DoctorId"),
                                PatientId = reader.SafeGetInt("PatientId"),
                                AppointmentId = reader.SafeGetInt("AppointmentId"),
                                Message = reader.SafeGetString("Message"),
                                CreatedAt = reader.SafeGetDateTime("CreatedAt")
                            });
                        }
                    }
                }
            }
            return list;
        }

        public int ExecuteWrite(string indicator, Notification n)
        {
            int process = -1;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, n);

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

        private void AddParameters(SqlCommand cmd, string indicator, Notification n)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@notification_id", n.NotificationId);
            cmd.Parameters.AddWithValue("@doctor_id", n.DoctorId);
            cmd.Parameters.AddWithValue("@pacient_id", n.PatientId);
            cmd.Parameters.AddWithValue("@appointment_id", n.AppointmentId);
            cmd.Parameters.AddWithValue("@message", (object)n.Message ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@is_read", n.IsRead);
            cmd.Parameters.AddWithValue("@role", string.IsNullOrEmpty(n.Role) ? (object)DBNull.Value : n.Role.ToLower());
        }
    }
}