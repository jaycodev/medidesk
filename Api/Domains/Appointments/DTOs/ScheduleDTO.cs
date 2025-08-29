namespace Api.Domains.Appointments.DTOs
{
    public class ScheduleDTO
    {
        public string DayWorkShift { get; set; } = string.Empty;
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
    }
}
