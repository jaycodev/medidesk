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
            int result = -1;

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, a);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    if (indicator == "INSERT")
                    {
                        object insertedId = cmd.ExecuteScalar();
                        result = insertedId != null ? Convert.ToInt32(insertedId) : -1;
                    }
                    else if (indicator == "UPDATE" || indicator == "CANCEL")
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                result = Convert.ToInt32(reader["affected_rows"]);
                        }
                    }
                    else
                    {
                        result = cmd.ExecuteNonQuery();
                    }
                }
            }

            return result;
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
            cmd.Parameters.AddWithValue("@user_type", a.UserType);
        }
    }
}