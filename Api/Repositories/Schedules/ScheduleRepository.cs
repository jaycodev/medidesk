using System.Data;
using Api.Helpers;
using Microsoft.Data.SqlClient;
using Shared.DTOs.Schedules.Requests;
using Shared.DTOs.Schedules.Responses;

namespace Api.Repositories.Schedules
{
    public class ScheduleRepository : BaseRepository, IScheduleRepository
    {
        private const string CrudCommand = "Schedule_CRUD";

        public ScheduleRepository(string connectionString) : base(connectionString) { }

        public List<ScheduleResponse> GetList(int idDoctor)
        {
            var currentSchedules = new List<ScheduleResponse>();

            using (SqlConnection cn = GetConnection())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(CrudCommand, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicator", "GET_BY_DOCTOR");
                    cmd.Parameters.AddWithValue("@doctor_id", idDoctor);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            currentSchedules.Add(new ScheduleResponse
                            {
                                DoctorId = reader.SafeGetInt("doctor_id"),
                                Weekday = reader.SafeGetString("weekday"),
                                DayWorkShift = reader.SafeGetString("day_work_shift"),
                                StartTime = reader.SafeGetTimeSpan("start_time"),
                                EndTime = reader.SafeGetTimeSpan("end_time"),
                                IsActive = true
                            });
                        }
                    }
                }
            }

            var weekDays = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };
            var dayWorkShifts = new[] { "mañana", "tarde" };

            var existingDict = currentSchedules
                .ToDictionary(h => $"{h.Weekday.ToLower()}_{h.DayWorkShift}", h => h);

            var fullSchedules = (
                from day in weekDays
                from shift in dayWorkShifts
                let key = $"{day.ToLower()}_{shift}"
                select existingDict.TryGetValue(key, out var existingShift)
                    ? new ScheduleResponse
                    {
                        DoctorId = existingShift.DoctorId,
                        Weekday = existingShift.Weekday,
                        DayWorkShift = existingShift.DayWorkShift,
                        StartTime = existingShift.StartTime,
                        EndTime = existingShift.EndTime,
                        IsActive = true
                    }
                    : new ScheduleResponse
                    {
                        DoctorId = idDoctor,
                        Weekday = day,
                        DayWorkShift = shift,
                        StartTime = TimeSpan.Zero,
                        EndTime = TimeSpan.Zero,
                        IsActive = false
                    }
            ).ToList();

            return fullSchedules;
        }

        public List<ScheduleByDateResponse> GetByDate(int doctorId, DateTime date)
        {
            var list = new List<ScheduleByDateResponse>();

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
                list.Add(new ScheduleByDateResponse
                {
                    DayWorkShift = reader.SafeGetString("day_work_shift"),
                    StartTime = reader.SafeGetTimeSpan("start_time"),
                    EndTime = reader.SafeGetTimeSpan("end_time")
                });
            }

            return list;
        }

        private int CreateOrUpdateProcedure(ScheduleRequest request)
        {
            int process = -1;
            using (SqlConnection cn = GetConnection())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(CrudCommand, cn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicator", "INSERT_OR_UPDATE");
                    cmd.Parameters.AddWithValue("@doctor_id", request.DoctorId);
                    cmd.Parameters.AddWithValue("@weekday", request.Weekday);
                    cmd.Parameters.AddWithValue("@day_work_shift", request.DayWorkShift);
                    cmd.Parameters.AddWithValue("@start_time", request.StartTime);
                    cmd.Parameters.AddWithValue("@end_time", request.EndTime);
                    cmd.Parameters.AddWithValue("@enabled", request.IsActive);

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

        public List<string> CreateOrUpdate(List<ScheduleRequest> requests)
        {
            var messages = new List<string>();

            for (int i = 0; i < requests.Count; i++)
            {
                var schedule = requests[i];

                if (schedule.IsActive && schedule.StartTime >= schedule.EndTime)
                {
                    messages.Add($"En el día {schedule.Weekday}, la hora de inicio debe ser menor que la hora final.");
                    continue;
                }

                if (i % 2 == 0 && i + 1 < requests.Count)
                {
                    var morningShift = requests[i];
                    var afternoonShift = requests[i + 1];

                    if (morningShift.IsActive && afternoonShift.IsActive)
                    {
                        bool overlap =
                            morningShift.EndTime > afternoonShift.StartTime ||
                            morningShift.StartTime >= afternoonShift.StartTime;

                        if (overlap)
                            messages.Add($"En el día {schedule.Weekday}, los horarios de mañana y tarde no deben cruzarse.");
                    }
                }
            }

            if (messages.Any())
                return messages;

            int totalAffected = requests.Sum(s => CreateOrUpdateProcedure(s));

            messages.Add(totalAffected > 0 ? "✅ ¡Horarios actualizados correctamente!" : "ℹ️ No se realizaron cambios.");

            return messages;
        }
    }
}
