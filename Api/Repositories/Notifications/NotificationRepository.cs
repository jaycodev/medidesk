using System.Data;
using Api.Helpers;
using Microsoft.Data.SqlClient;
using Shared.DTOs.Notifications;

namespace Api.Repositories.Notifications
{
    public class NotificationRepository : BaseRepository, INotificationRepository
    {
        private const string CrudCommand = "Notification_CRUD";

        public NotificationRepository(string connectionString) : base(connectionString) { }

        public List<NotificationResponse> GetForDoctor(int doctorId)
        {
            var list = new List<NotificationResponse>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL_DOC_NOTIFICATIONS");
            cmd.Parameters.AddWithValue("@doctor_id", doctorId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new NotificationResponse
                {
                    NotificationId = reader.SafeGetInt("NotificationId"),
                    DoctorId = reader.SafeGetInt("DoctorId"),
                    PatientId = reader.SafeGetInt("PatientId"),
                    AppointmentId = reader.SafeGetInt("AppointmentId"),
                    Message = reader.SafeGetString("Message"),
                    CreatedAt = reader.SafeGetDateTime("CreatedAt")
                });
            }
            return list;
        }

        public List<NotificationResponse> GetForPatient(int patientId)
        {
            var list = new List<NotificationResponse>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL_PA_NOTIFICATIONS");
            cmd.Parameters.AddWithValue("@patient_id", patientId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new NotificationResponse
                {
                    NotificationId = reader.SafeGetInt("NotificationId"),
                    DoctorId = reader.SafeGetInt("DoctorId"),
                    PatientId = reader.SafeGetInt("PatientId"),
                    AppointmentId = reader.SafeGetInt("AppointmentId"),
                    Message = reader.SafeGetString("Message"),
                    CreatedAt = reader.SafeGetDateTime("CreatedAt")
                });
            }
            return list;
        }

        public int Delete(int id)
        {
            using var cn = GetConnection();
            cn.Open();

            using (var check = new SqlCommand("SELECT 1 FROM Notifications WHERE NotificationId = @id", cn))
            {
                check.Parameters.AddWithValue("@id", id);
                var exists = check.ExecuteScalar();
                if (exists is null)
                    return 0;
            }

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@indicator", "DELETE_BY_ID");
            cmd.Parameters.AddWithValue("@notification_id", id);
            cmd.ExecuteNonQuery();

            return 1;
        }
    }
}