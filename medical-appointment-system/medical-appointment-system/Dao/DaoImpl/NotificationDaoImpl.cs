using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using DocumentFormat.OpenXml.Office2010.PowerPoint;
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
                using (SqlCommand cmd = new SqlCommand(crudCommand))
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
                        process = cmd.ExecuteNonQuery();
                    }
                    catch (SqlException ex)
                    {
                        if(ex.Number == 5000)
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


        private void AddParameters(SqlCommand cmd, string indicator, Notification n)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@notificationId", n.NotificationId);
            cmd.Parameters.AddWithValue("@doctor_id", n.DoctorId);
            cmd.Parameters.AddWithValue("@patient_id", n.PatientId);
            cmd.Parameters.AddWithValue("@appointmentId", n.AppointmentId);
            cmd.Parameters.AddWithValue("@message", n.Message);
            cmd.Parameters.AddWithValue("@isRead", n.IsRead);
            cmd.Parameters.AddWithValue("@createdAt", n.CreatedAt);
        }
    }
}