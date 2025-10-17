using System.Data;
using Api.Extensions;
using Api.Helpers;
using Api.Queries;
using Microsoft.Data.SqlClient;
using Shared.DTOs.Appointments.Requests;
using Shared.DTOs.Appointments.Responses;

namespace Api.Repositories.Appointments
{
    public class AppointmentRepository : BaseRepository, IAppointmentRepository
    {
        private const string CrudCommand = "Appointment_CRUD";

        public AppointmentRepository(string connectionString) : base(connectionString) { }

        public List<AppointmentListResponse> GetAll(ListQuery listQuery, AppointmentQuery query)
        {
            var list = new List<AppointmentListResponse>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            // TODO: Invoke AddWithValue based on the query properties that are not null
            cmd.Parameters.AddWithValue("@indicator", "GET_ALL");
            cmd.Parameters.AddQueryAsParameters(query);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new AppointmentListResponse
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

        public List<AppointmentListResponse> GetAppointmentsByStatus(AppointmentQuery query)
        {
            var list = new List<AppointmentListResponse>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_USER_AND_STATUS");
            cmd.Parameters.AddQueryAsParameters(query);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new AppointmentListResponse
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

        public List<AppointmentListResponse> GetHistorial(int userId, string userRol)
        {
            var list = new List<AppointmentListResponse>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_COMPLETED_OR_CANCELLED_BY_USER");
            cmd.Parameters.AddWithValue("@user_id", userId);
            cmd.Parameters.AddWithValue("@user_rol", userRol);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new AppointmentListResponse
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

        public AppointmentResponse? GetById(int id)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_ID");
            cmd.Parameters.AddWithValue("@appointment_id", id);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new AppointmentResponse
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

        public List<AppointmentTimeResponse> GetByDoctorAndDate(int doctorId, DateTime date)
        {
            var list = new List<AppointmentTimeResponse>();

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", "GET_BY_DOCTOR_AND_DATE");
            cmd.Parameters.AddWithValue("@doctor_id", doctorId);
            cmd.Parameters.AddWithValue("@date", date.Date);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new AppointmentTimeResponse
                {
                    Time = reader.SafeGetTimeSpan("time")
                });
            }

            return list;
        }

        public int Reserve(CreateAppointmentRequest request)
        {
            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();

            cmd.Parameters.AddWithValue("@indicator", "INSERT");
            cmd.Parameters.AddWithValue("@doctor_id", request.DoctorId);
            cmd.Parameters.AddWithValue("@patient_id", request.PatientId);
            cmd.Parameters.AddWithValue("@specialty_id", request.SpecialtyId);
            cmd.Parameters.AddWithValue("@date", request.Date);
            cmd.Parameters.AddWithValue("@time", request.Time);
            cmd.Parameters.AddWithValue("@consultation_type", request.ConsultationType);
            cmd.Parameters.AddWithValue("@symptoms", request.Symptoms);
            cmd.Parameters.AddWithValue("@status", "pendiente");

            var newId = cmd.ExecuteScalar();
            return newId != null ? Convert.ToInt32(newId) : -1;
        }

        public int Update(int id, UpdateAppointmentRequest request)
        {
            string indicator = request.Status switch
            {
                "confirmada" => "CONFIRM",
                "cancelada" => "CANCEL",
                "atendida" => "ATTEND",
                "pendiente" => throw new ArgumentException("No existe transición a 'pendiente'"),
                _ => throw new ArgumentException("Estado no soportado")
            };

            using var cn = GetConnection();
            cn.Open();

            using var cmd = new SqlCommand(CrudCommand, cn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@appointment_id", id);

            var result = cmd.ExecuteScalar();
            return result == null || result == DBNull.Value ? -1 : Convert.ToInt32(result);
        }
    }
}