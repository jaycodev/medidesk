using Api.Data.Repository;
using Api.Domains.Appointments.DTOs;
using Api.Extensions;
using Api.Helpers;
using Api.Queries;
using Microsoft.Data.SqlClient;
using System.Data;

namespace Api.Domains.Appointments.Repositories
{
    public class AppointmentRepository : BaseRepository, IAppointmentRepository
    {
        string crudCommand = "Appointment_CRUD";

        public AppointmentRepository(IConfiguration configuration) : base(configuration) { }

        public List<AppointmentListDTO> GetAll(ListQuery listQuery, AppointmentQuery query)
        {
            var list = new List<AppointmentListDTO>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            // TODO: Invoke AddWithValue based on the query properties that are not null
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");
            cmd.Parameters.AddQueryAsParameters(query);

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

        public List<AppointmentListDTO> GetAppointmentsByStatus(AppointmentQuery query)
        {
            var list = new List<AppointmentListDTO>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_USER_AND_STATUS");
            cmd.Parameters.AddQueryAsParameters(query);

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

        public List<AppointmentListDTO> GetHistorial(int userId, string userRol)
        {
            var list = new List<AppointmentListDTO>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_COMPLETED_OR_CANCELLED_BY_USER");
            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@user_rol", userRol);

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

        public List<AppointmentTimeDTO> GetByDoctorAndDate(int doctorId, DateTime date)
        {
            var list = new List<AppointmentTimeDTO>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_DOCTOR_AND_DATE");
            cmd.Parameters.AddWithValue("@doctor_id", doctorId);
            cmd.Parameters.AddWithValue("@date", date.Date);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new AppointmentTimeDTO
                {
                    Time = reader.SafeGetTimeSpan("time")
                });
            }

            return list;
        }

        public List<ScheduleDTO> GetScheduleByDoctorAndDay(int doctorId, DateTime date)
        {
            var list = new List<ScheduleDTO>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_SCHEDULE_BY_DOCTOR_AND_DAY");
            cmd.Parameters.AddWithValue("@doctor_id", doctorId);
            cmd.Parameters.AddWithValue("@date", date.Date);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new ScheduleDTO
                {
                    DayWorkShift = reader.SafeGetString("day_work_shift"),
                    StartTime = reader.SafeGetTimeSpan("start_time"),
                    EndTime = reader.SafeGetTimeSpan("end_time")
                });
            }

            return list;
        }

        public int Reserve(CreateAppointmentDTO dto)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(crudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@indicator", "INSERT");
            cmd.Parameters.AddWithValue("@doctor_id", dto.DoctorId);
            cmd.Parameters.AddWithValue("@patient_id", dto.PatientId);
            cmd.Parameters.AddWithValue("@specialty_id", dto.SpecialtyId);
            cmd.Parameters.AddWithValue("@date", dto.Date);
            cmd.Parameters.AddWithValue("@time", dto.Time);
            cmd.Parameters.AddWithValue("@consultation_type", dto.ConsultationType);
            cmd.Parameters.AddWithValue("@symptoms", dto.Symptoms);
            cmd.Parameters.AddWithValue("@status", "pendiente");

            var newId = cmd.ExecuteScalar();
            return newId != null ? Convert.ToInt32(newId) : -1;
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