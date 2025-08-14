using Api.Data.Contract;
using Api.Helpers;
using Api.Models;
using Microsoft.Data.SqlClient;

namespace Api.Data.Repository
{
    public class AppointmentRepository : BaseRepository, IGenericContract<Appointment>
    {
        string crudCommand = "Appointment_CRUD";
        public AppointmentRepository(IConfiguration configuration) : base(configuration) { }
        public List<Appointment> ExecuteRead(string indicator, Appointment a)
        {
            List<Appointment> list = new List<Appointment>();
            using (var cn = GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, a);
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Appointment
                            {
                                AppointmentId = reader.SafeGetInt("appointment_id"),
                                DoctorId = reader.SafeGetInt("doctor_id"),
                                DoctorName = reader.SafeGetString("doctor_name"),
                                PatientId = reader.SafeGetInt("patient_id"),
                                PatientName = reader.SafeGetString("patient_name"),
                                SpecialtyId = reader.SafeGetInt("specialty_id"),
                                SpecialtyName = reader.SafeGetString("specialty_name"),
                                Date = reader.SafeGetDateTime("date"),
                                Time = reader.SafeGetTimeSpan("time"),
                                ConsultationType = reader.SafeGetString("consultation_type"),
                                Symptoms = reader.SafeGetString("symptoms"),
                                Status = reader.SafeGetString("status"),
                                UserId = reader.SafeGetInt("user_id"),
                                UserRol = reader.SafeGetString("user_rol"),
                                DayWorkShift = reader.SafeGetString("day_work_shift")
                            });
                        }
                    }
                }
            }
            return list;
        }
        public int ExecuteWrite(string indicator, Appointment a)
        {
            int process = -1;
            using (var cn = GetConnection())
            {
                cn.Open();
                using (var cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, a);
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
                        throw new Exception(ex.Message);
                    }
                }
            }
            return process;
        }

        private void AddParameters(SqlCommand cmd, string indicator, Appointment a)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@appointment_id", a.AppointmentId);
            cmd.Parameters.AddWithValue("@doctor_id", a.DoctorId);
            cmd.Parameters.AddWithValue("@doctor_name", a.DoctorName);
            cmd.Parameters.AddWithValue("@patient_id", a.PatientId);
            cmd.Parameters.AddWithValue("@patient_name", a.PatientName);
            cmd.Parameters.AddWithValue("@specialty_id", a.SpecialtyId);
            cmd.Parameters.AddWithValue("@specialty_name", a.SpecialtyName);
            cmd.Parameters.AddWithValue("@date", a.Date);
            cmd.Parameters.AddWithValue("@time", a.Time);
            cmd.Parameters.AddWithValue("@consultation_type", a.ConsultationType);
            cmd.Parameters.AddWithValue("@symptoms", a.Symptoms);
            cmd.Parameters.AddWithValue("@status", a.Status);
            cmd.Parameters.AddWithValue("@user_id", a.UserId);
            cmd.Parameters.AddWithValue("@user_rol", a.UserRol);
            cmd.Parameters.AddWithValue("@day_work_shift", a.DayWorkShift);
        }
    }
}