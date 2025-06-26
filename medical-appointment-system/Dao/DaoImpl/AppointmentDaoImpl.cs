using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using medical_appointment_system.Helpers;
using medical_appointment_system.Models;

namespace medical_appointment_system.Dao.DaoImpl
{
    public class AppointmentDaoImpl : IGenericDao<Appointment>
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string crudCommand = "Appointment_CRUD";

        public int ExecuteWrite(string indicator, Appointment a)
        {
            int process = -1;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, a);

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

        public List<Appointment> ExecuteRead(string indicator, Appointment a)
        {
            List<Appointment> list = new List<Appointment>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, a);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Appointment
                            {
                                AppointmentId = reader.SafeGetInt("appointment_id"),
                                SpecialtyName = reader.SafeGetString("specialty_name"),
                                DoctorName = reader.SafeGetString("doctor_name"),
                                PatientName = reader.SafeGetString("patient_name"),
                                ConsultationType = reader.SafeGetString("consultation_type"),
                                Date = reader.SafeGetDateTime("date"),
                                Time = reader.SafeGetTimeSpan("time"),
                                Symptoms = reader.SafeGetString("symptoms"),
                                Status = reader.SafeGetString("status"),
                                DayWorkShift = reader.SafeGetString("day_work_shift"),
                                StartTime = reader.SafeGetTimeSpan("start_time"),
                                EndTime = reader.SafeGetTimeSpan("end_time")
                            });
                        }
                    }
                }
            }
            return list;
        }

        private void AddParameters(SqlCommand cmd, string indicator, Appointment a)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@appointment_id", a.AppointmentId);
            cmd.Parameters.AddWithValue("@doctor_id", a.DoctorId);
            cmd.Parameters.AddWithValue("@patient_id", a.PatientId);
            cmd.Parameters.AddWithValue("@specialty_id", a.SpecialtyId);
            cmd.Parameters.AddWithValue("@date", a.Date);
            cmd.Parameters.AddWithValue("@time", a.Time);
            cmd.Parameters.AddWithValue("@consultation_type", a.ConsultationType);
            cmd.Parameters.AddWithValue("@symptoms", a.Symptoms);
            cmd.Parameters.AddWithValue("@status", a.Status);
            cmd.Parameters.AddWithValue("@user_id", a.UserId);
            cmd.Parameters.AddWithValue("@user_rol", a.UserRol);
        }
    }
}