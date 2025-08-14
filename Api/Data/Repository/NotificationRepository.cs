using Api.Data.Contract;
using Api.Helpers;
using Api.Models;
using Microsoft.Data.SqlClient;

namespace Api.Data.Repository
{
    public class NotificationRepository : BaseRepository, IGenericContract<Notification>
    {
        string crudCommand = "Notification_CRUD";
        public NotificationRepository(IConfiguration configuration) : base(configuration) { }
        public List<Notification> ExecuteRead(string indicator, Notification n)
        {
            List<Notification> list = new List<Notification>();
            using (var cn = GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, n);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Notification
                            {
                                NotificationId = reader.SafeGetInt("notification_id"),
                                DoctorId = reader.SafeGetInt("doctor_id"),
                                PatientId = reader.SafeGetInt("patient_id"),
                                AppointmentId = reader.SafeGetInt("appointment_id"),
                                Message = reader.SafeGetString("message"),
                                IsRead = reader.SafeGetBool("is_read"),
                                CreatedAt = reader.SafeGetDateTime("created_at"),
                                Role = reader.SafeGetString("role")
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
            using (var cn = GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, n);
                    try
                    {
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                process = reader.SafeGetInt("process");
                            }
                        }
                    }
                    catch (SqlException ex)
                    {
                        // Log exception details here
                        throw new Exception("An error occurred while executing the database operation.", ex);
                    }
                }
            }
            return process;
        }

        private void AddParameters(SqlCommand cmd, string indicator, Notification n)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@notification_id", n.NotificationId);
            cmd.Parameters.AddWithValue("@doctor_id", n.DoctorId.HasValue ? (object)n.DoctorId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@patient_id", n.PatientId.HasValue ? (object)n.PatientId.Value : DBNull.Value);
            cmd.Parameters.AddWithValue("@appointment_id", n.AppointmentId);
            cmd.Parameters.AddWithValue("@message", n.Message ?? (object)DBNull.Value);
            cmd.Parameters.AddWithValue("@is_read", n.IsRead);
            cmd.Parameters.AddWithValue("@created_at", n.CreatedAt != DateTime.MinValue ? (object)n.CreatedAt : DBNull.Value);
            cmd.Parameters.AddWithValue("@role", n.Role ?? (object)DBNull.Value);
        }
    }
}