using Api.Data.Repository;
using Api.Domains.Schedules.DTOs;
using Api.Domains.Schedules.Models;
using Api.Helpers;
using Microsoft.Data.SqlClient;

namespace Api.Domains.Schedules.Repositories
{
    public class ScheduleRepository : BaseRepository, ISchedule
    {
        public ScheduleRepository(IConfiguration configuration) : base(configuration) { }

        string crudCommand = "Schedule_CRUD";

        public int CreateOrUpdateScheduleProcedure(ScheduleDTO s)
        {
            int process = -1;
            using (SqlConnection cn = GetConnection())
            {
                cn.Open();
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;
                    cmd.Parameters.Clear();
                    cmd.Parameters.AddWithValue("@indicator", "INSERT_OR_UPDATE");
                    cmd.Parameters.AddWithValue("@doctor_id", s.DoctorId);
                    cmd.Parameters.AddWithValue("@weekday", s.Weekday);
                    cmd.Parameters.AddWithValue("@day_work_shift", s.DayWorkShift);
                    cmd.Parameters.AddWithValue("@start_time", s.StartTime);
                    cmd.Parameters.AddWithValue("@end_time", s.EndTime);
                    cmd.Parameters.AddWithValue("@enabled", s.IsActive);

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


        public List<string> CreateOrUpdateSchedules(List<ScheduleDTO> schedules)
        {
            var messages = new List<string>();

            // 🔹 VALIDACIONES
            for (int i = 0; i < schedules.Count; i++)
            {
                var schedule = schedules[i];

                // 1️⃣ Validar que la hora de inicio sea menor que la hora fin
                if (schedule.IsActive && schedule.StartTime >= schedule.EndTime)
                {
                    messages.Add($"En el día {schedule.Weekday}, la hora de inicio debe ser menor que la hora final.");
                    continue;
                }

                // 2️⃣ Validar que los turnos de mañana y tarde no se crucen
                // Cada día tiene 2 turnos: posición par = mañana, posición impar = tarde
                if (i % 2 == 0 && i + 1 < schedules.Count)
                {
                    var morningShift = schedules[i];
                    var afternoonShift = schedules[i + 1];

                    if (morningShift.IsActive && afternoonShift.IsActive)
                    {
                        bool overlap =
                            morningShift.EndTime > afternoonShift.StartTime ||   // La mañana termina después de que empieza la tarde
                            morningShift.StartTime >= afternoonShift.StartTime;  // La mañana empieza igual o después de la tarde

                        if (overlap)
                        {
                            messages.Add($"En el día {schedule.Weekday}, los horarios de mañana y tarde no deben cruzarse.");
                        }
                    }
                }
            }

            // 🔹 Si hubo errores → se devuelven y no se guarda nada
            if (messages.Any())
                return messages;

            // 🔹 Guardar en BD (llamando al SP para cada horario)
            int totalAffected = schedules.Sum(s => CreateOrUpdateScheduleProcedure(s));

            // 🔹 Mensaje de resultado
            messages.Add(totalAffected > 0
                ? "✅ ¡Horarios actualizados correctamente!"
                : "ℹ️ No se realizaron cambios.");

            return messages;
        }

        // Método que devuelve una lista de horarios (ScheduleDTO) de un doctor en particular
        public List<ScheduleDTO> GetListSchedulesByIdDoctor(int idDoctor)
        {
            // Lista temporal que almacenará los horarios obtenidos desde la base de datos
            List<Schedule> currentSchedules = new List<Schedule>();

            // Abrimos la conexión a la base de datos
            using (SqlConnection cn = GetConnection())
            {
                cn.Open();

                // Se prepara el comando SQL que ejecutará un procedimiento almacenado (Stored Procedure)
                using (SqlCommand cmd = new SqlCommand(crudCommand, cn))
                {
                    cmd.CommandType = System.Data.CommandType.StoredProcedure; // Indicamos que es un SP
                    cmd.Parameters.Clear(); // Limpiamos parámetros previos (por seguridad)

                    // Parámetros que necesita el procedimiento almacenado
                    cmd.Parameters.AddWithValue("@indicator", "GET_BY_DOCTOR"); // Acción a ejecutar
                    cmd.Parameters.AddWithValue("@doctor_id", idDoctor);        // ID del doctor a consultar

                    // Ejecutamos el comando y leemos los resultados
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        // Mientras haya registros
                        while (reader.Read())
                        {
                            // Creamos un objeto Schedule con los datos del registro leído
                            currentSchedules.Add(new Schedule
                            {
                                DoctorId = reader.SafeGetInt("doctor_id"),             // ID del doctor
                                Weekday = reader.SafeGetString("weekday"),             // Día de la semana
                                DayWorkShift = reader.SafeGetString("day_work_shift"), // Turno (mañana/tarde)
                                StartTime = reader.SafeGetTimeSpan("start_time"),      // Hora inicio
                                EndTime = reader.SafeGetTimeSpan("end_time")           // Hora fin
                            });
                        }
                    }
                }
            }

            // Lista final que vamos a retornar, con los horarios "normalizados"
            List<ScheduleDTO> fullSchedules = new List<ScheduleDTO>();

            // Si se encontraron horarios en la base de datos
            if (true)
            {
                // Lista de todos los días de la semana
                var weekDays = new[] { "Lunes", "Martes", "Miércoles", "Jueves", "Viernes", "Sábado", "Domingo" };

                // Lista de todos los turnos disponibles
                var dayWorkShifts = new[] { "mañana", "tarde" };

                // Convertimos la lista de horarios en un diccionario
                // Clave = "día_turno" en minúsculas (ejemplo: "lunes_mañana")
                // Valor = el objeto Schedule correspondiente
                var existingDict = currentSchedules
                    .ToDictionary(h => $"{h.Weekday.ToLower()}_{h.DayWorkShift}", h => h);

                // Recorremos todos los días y turnos posibles
                fullSchedules = (
                    from day in weekDays
                    from shift in dayWorkShifts
                    let key = $"{day.ToLower()}_{shift}" // Creamos la clave para validar si existe
                    select existingDict.TryGetValue(key, out var existingShift)
                        ? new ScheduleDTO // Si existe en BD, devolvemos el horario activo
                        {
                            DoctorId = existingShift.DoctorId,
                            Weekday = existingShift.Weekday,
                            DayWorkShift = existingShift.DayWorkShift,
                            StartTime = existingShift.StartTime,
                            EndTime = existingShift.EndTime,
                            IsActive = true // Existe en la BD
                        }
                        : new ScheduleDTO // Si no existe en BD, devolvemos un horario vacío/inactivo
                        {
                            DoctorId = idDoctor,
                            Weekday = day,
                            DayWorkShift = shift,
                            StartTime = TimeSpan.Zero, // Hora vacía
                            EndTime = TimeSpan.Zero,   // Hora vacía
                            IsActive = false           // No existe en la BD
                        }
                ).ToList(); // Convertimos el resultado en lista
            }

            // Retornamos la lista final (con todos los días y turnos normalizados)
            return fullSchedules;
        }


    }
}
