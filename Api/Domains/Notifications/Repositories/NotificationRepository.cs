using System.Data;
using Api.Data.Repository;
using Api.Domains.Notifications.DTOs;
using Api.Helpers;
using Microsoft.Data.SqlClient;

namespace Api.Domains.Notifications.Repositories
{
    public class NotificationRepository : BaseRepository, INotificationRepository
    {
        private const string crudCommand = "Notification_CRUD";
        public NotificationRepository(IConfiguration configuration) : base(configuration) { }

        public List<NotificationListDTO> GetForDoctor(int doctorId)
        {
            var list = new List<NotificationListDTO>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL_DOC_NOTIFICATIONS");
            cmd.Parameters.AddWithValue("@doctor_id", doctorId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new NotificationListDTO
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

        public List<NotificationListDTO> GetForPatient(int patientId)
        {
            var list = new List<NotificationListDTO>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL_PA_NOTIFICATIONS");
            cmd.Parameters.AddWithValue("@patient_id", patientId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new NotificationListDTO
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

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@indicator", "DELETE_BY_ID");
            cmd.Parameters.AddWithValue("@notification_id", id);
            cmd.ExecuteNonQuery();

            return 1;
        }
    }
}