using System.Data;
using System.Globalization;
using Api.Data.Repository;
using Api.Domains.Appointments.DTOs;
using Api.Helpers;
using Microsoft.Data.SqlClient;

namespace Api.Domains.Appointments.Repositories
{
    public class AppointmentRepository : BaseRepository, IAppointmentRepository
    {
        string crudCommand = "Appointment_CRUD";

        public AppointmentRepository(IConfiguration configuration) : base(configuration) { }

        public List<AppointmentListDTO> GetAll()
        {
            var list = new List<AppointmentListDTO>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new AppointmentListDTO
                {
                    AppointmentId = reader.SafeGetInt("appointment_id"),
                    SpecialtyName = reader.SafeGetString("specialty_name"),
                    DoctorName = reader.SafeGetString("doctor_name"),
                    PatientName = reader.SafeGetString("patient_name"),
                    ConsultationType = reader.SafeGetString("consultation_type"),
                    Date = reader.SafeGetDateOnly("date"),
                    Time = reader.SafeGetTimeSpan("time"),
                    Status = reader.SafeGetString("status")
                });
            }

            return list;
        }

        public List<AppointmentListDTO> GetAppointmentsByStatus(int userId, string userRol, string status)
        {
            var list = new List<AppointmentListDTO>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_USER_AND_STATUS");
            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@user_rol", userRol);
            cmd.Parameters.AddWithValue("@status", status);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new AppointmentListDTO
                {
                    AppointmentId = reader.SafeGetInt("appointment_id"),
                    SpecialtyName = reader.SafeGetString("specialty_name"),
                    DoctorName = reader.SafeGetString("doctor_name"),
                    PatientName = reader.SafeGetString("patient_name"),
                    ConsultationType = reader.SafeGetString("consultation_type"),
                    Date = reader.SafeGetDateOnly("date"),
                    Time = reader.SafeGetTimeSpan("time"),
                    Status = reader.SafeGetString("status")
                });
            }

            return list;
        }

        public AppointmentDetailDTO? GetById(int id)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_ID");
            cmd.Parameters.AddWithValue("@appointment_id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new AppointmentDetailDTO
                {
                    AppointmentId = reader.SafeGetInt("appointment_id"),
                    SpecialtyName = reader.SafeGetString("specialty_name"),
                    DoctorName = reader.SafeGetString("doctor_name"),
                    PatientName = reader.SafeGetString("patient_name"),
                    ConsultationType = reader.SafeGetString("consultation_type"),
                    Date = reader.SafeGetDateOnly("date"),
                    Time = reader.SafeGetTimeSpan("time"),
                    Status = reader.SafeGetString("status"),
                    Symptoms = reader.SafeGetString("symptoms")
                };
            }

            return null;
        }

        public int Create(CreateAppointmentDTO dto)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            var date = DateTime.ParseExact(dto.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var time = TimeSpan.ParseExact(dto.Time, @"hh\:mm", CultureInfo.InvariantCulture);
            cmd.Parameters.AddWithValue("@indicator", "INSERT");
            cmd.Parameters.AddWithValue("@doctor_id", dto.DoctorId);
            cmd.Parameters.AddWithValue("@patient_id", dto.PatientId);
            cmd.Parameters.AddWithValue("@specialty_id", dto.SpecialtyId);
            cmd.Parameters.Add("@date", SqlDbType.Date).Value = date.Date;
            cmd.Parameters.Add("@time", SqlDbType.Time).Value = time;
            cmd.Parameters.AddWithValue("@consultation_type", dto.ConsultationType);
            cmd.Parameters.AddWithValue("@symptoms", (object?)dto.Symptoms ?? DBNull.Value);
            cmd.Parameters.AddWithValue("@status", "pendiente");

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
                return reader.SafeGetInt("appointment_id");

            return -1;
        }

        public int Update(int id, UpdateAppointmentStatusDTO dto)
        {
            string indicator = dto.Status switch
            {
                "confirmada" => "CONFIRM",
                "cancelada" => "CANCEL",
                "atendida" => "ATTEND",
                "pendiente" => throw new ArgumentException("No existe transición a 'pendiente'"),
                _ => throw new ArgumentException("Estado no soportado")
            };

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@appointment_id", id);

            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? -1 : Convert.ToInt32(result);
        }
    }
}