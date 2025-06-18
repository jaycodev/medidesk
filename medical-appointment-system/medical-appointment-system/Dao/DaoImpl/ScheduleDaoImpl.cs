using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using medical_appointment_system.Helpers;
using medical_appointment_system.Models;

namespace medical_appointment_system.Dao.DaoImpl
{
    public class ScheduleDaoImpl : IGenericDao<Schedule>
    {
        string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        string crudCommand = "Schedule_CRUD";

        public int ExecuteWrite(string indicator, Schedule s)
        {
            int process;
            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, s);

                    process = cmd.ExecuteNonQuery();
                }
            }
            return process;
        }

        public List<Schedule> ExecuteRead(string indicator, Schedule s)
        {
            List<Schedule> list = new List<Schedule>();

            using (SqlConnection cn = new SqlConnection(connectionString))
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    AddParameters(cmd, indicator, s);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            list.Add(new Schedule
                            {
                                ScheduleId = reader.SafeGetInt("schedule_id"),
                                DoctorId = reader.SafeGetInt("doctor_id"),
                                Weekday = reader.SafeGetString("weekday"),
                                StartTime = reader.SafeGetTimeSpan("start_time"),
                                EndTime = reader.SafeGetTimeSpan("end_time")
                            });
                        }
                    }
                }
            }
            return list;
        }

        private void AddParameters(SqlCommand cmd, string indicator, Schedule s)
        {
            cmd.CommandType = System.Data.CommandType.StoredProcedure;
            cmd.Parameters.Clear();
            cmd.Parameters.AddWithValue("@indicator", indicator);
            cmd.Parameters.AddWithValue("@schedule_id", s.ScheduleId);
            cmd.Parameters.AddWithValue("@doctor_id", s.DoctorId);
            cmd.Parameters.AddWithValue("@weekday", s.Weekday);
            cmd.Parameters.AddWithValue("@start_time", s.StartTime);
            cmd.Parameters.AddWithValue("@end_time", s.EndTime);
        }
    }
}
